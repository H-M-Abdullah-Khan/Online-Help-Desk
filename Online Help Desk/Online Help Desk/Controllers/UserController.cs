using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Help_Desk.Models;

namespace Online_Help_Desk.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsEndUser()
        {
            return HttpContext.Session.GetString("Role") == RoleEnum.EndUser.ToString();
        }

        private int GetUserId()
        {
            return HttpContext.Session.GetInt32("UserId") ?? 0;
        }

        // 🧠 USER DASHBOARD
        public IActionResult Dashboard()
        {
            if (!IsEndUser()) return RedirectToAction("Login", "Auth");

            int uid = GetUserId();
            var user = _context.Users.FirstOrDefault(u => u.UserId == uid);
            HttpContext.Session.SetString("Username", user?.FullName ?? "User");
            var currentUser = _context.Users.FirstOrDefault(u => u.UserId == uid);
            ViewBag.Username = currentUser?.FullName ?? "User";


            var userRequests = _context.Requests
                .Include(r => r.Facility)
                .Where(r => r.UserId == uid)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            ViewBag.RequestCount = userRequests.Count;
            ViewBag.ResolvedCount = userRequests.Count(r => r.Status == RequestStatus.Closed);
            ViewBag.PendingCount = userRequests.Count(r => r.Status != RequestStatus.Closed);

            return View(userRequests); // Loop over @model in Dashboard.cshtml
        }

        // 🧠 NEW REQUEST (GET)
        public IActionResult NewRequest()
        {
            if (!IsEndUser()) return RedirectToAction("Login", "Auth");
            int uid = GetUserId();
            var currentUser = _context.Users.FirstOrDefault(u => u.UserId == uid);
            ViewBag.Username = currentUser?.FullName ?? "User";


            ViewBag.Facilities = _context.Facilities.ToList(); // for dropdown
            return View();
        }

        // 🧠 NEW REQUEST (POST)
        [HttpPost]
        public IActionResult NewRequest(Request model)
        {
            if (!IsEndUser()) return RedirectToAction("Login", "Auth");

            int uid = GetUserId();
            var currentUser = _context.Users.FirstOrDefault(u => u.UserId == uid);
            ViewBag.Username = currentUser?.FullName ?? "User";

            model.UserId = uid;

            model.Status = RequestStatus.Pending;
            model.CreatedAt = DateTime.Now;
            model.AssignedToUserId = null;

            _context.Requests.Add(model);
            _context.SaveChanges();

            TempData["RequestSubmitted"] = "Your request has been submitted successfully!";
            return RedirectToAction("TrackRequests");
        }

        // 🧠 TRACK USER REQUESTS
        public IActionResult TrackRequests()
        {
            if (!IsEndUser()) return RedirectToAction("Login", "Auth");

            int uid = GetUserId();
            var currentUser = _context.Users.FirstOrDefault(u => u.UserId == uid);
            ViewBag.Username = currentUser?.FullName ?? "User";

            var requests = _context.Requests
                .Include(r => r.Facility)
                .Where(r => r.UserId == uid)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            ViewBag.Total = requests.Count;
            ViewBag.Pending = requests.Count(r => r.Status == RequestStatus.Pending || r.Status == RequestStatus.Assigned);
            ViewBag.Resolved = requests.Count(r => r.Status == RequestStatus.Closed);

            return View(requests);
        }


        // 🧠 CLOSE OWN REQUEST
        public IActionResult CloseRequest(int id)
        {
            if (!IsEndUser()) return RedirectToAction("Login", "Auth");

            int uid = GetUserId();
            var currentUser = _context.Users.FirstOrDefault(u => u.UserId == uid);
            ViewBag.Username = currentUser?.FullName ?? "User";
            var req = _context.Requests.FirstOrDefault(r => r.RequestId == id && r.UserId == uid);

            if (req != null && req.Status != RequestStatus.Closed)
            {
                req.Status = RequestStatus.Closed;
                req.UpdatedAt = DateTime.Now;

                _context.StatusHistories.Add(new StatusHistory
                {
                    RequestId = req.RequestId,
                    UpdatedByUserId = uid,
                    Status = RequestStatus.Closed,
                    UpdatedAt = DateTime.Now
                });

                _context.SaveChanges();
                TempData["RequestClosed"] = "Your request was closed successfully!";
            }


            return RedirectToAction("TrackRequests");
        }
        public IActionResult Profile()
        {
            int uid = HttpContext.Session.GetInt32("UserId") ?? 0;
            var user = _context.Users.FirstOrDefault(u => u.UserId == uid);
            if (user == null) return RedirectToAction("Login", "Auth");
            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateProfile(User updatedUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == updatedUser.UserId);
            if (user != null)
            {
                user.FullName = updatedUser.FullName;
                user.Username = updatedUser.Username;
                user.Email = updatedUser.Email;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "User not found!";
            }
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public IActionResult ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found!";
                return RedirectToAction("Profile");
            }

            if (user.PasswordHash != currentPassword)
            {
                TempData["ErrorMessage"] = "Current password is incorrect.";
                return RedirectToAction("Profile");
            }

            user.PasswordHash = newPassword;
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("Profile");
        }

    }
}
