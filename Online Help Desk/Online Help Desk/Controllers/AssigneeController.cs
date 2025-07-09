using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Help_Desk.Models;

namespace Online_Help_Desk.Controllers
{
    public class AssigneeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AssigneeController(ApplicationDbContext context) => _context = context;

        private bool IsAssignee()
        {
            return HttpContext.Session.GetString("Role") == RoleEnum.Assignee.ToString();
        }

        private int GetUserId()
        {
            return HttpContext.Session.GetInt32("UserId") ?? 0;
        }

        // Dashboard for Assignee
        public IActionResult Dashboard()
        {
            if (!IsAssignee()) return RedirectToAction("Login", "Auth");

            int uid = GetUserId();
            var currentUser = _context.Users.FirstOrDefault(u => u.UserId == uid);
            ViewBag.Username = currentUser?.FullName ?? "Assignee";

            //var assigned = _context.Requests.Where(r => r.AssignedToUserId == uid);
            var assigned = _context.Requests
            .Include(r => r.User)
            .Include(r => r.Facility)
            .Where(r => r.AssignedToUserId == uid);

            ViewBag.TotalTasks = assigned.Count();
            ViewBag.InProgress = assigned.Count(r => r.Status == RequestStatus.InProgress);
            ViewBag.Pending = assigned.Count(r => r.Status == RequestStatus.Assigned);
            ViewBag.Completed = assigned.Count(r => r.Status == RequestStatus.Closed);

            return View(assigned.ToList());
        }


        // 📋 View My Assigned Tasks
        public IActionResult MyTasks()
        {
            if (!IsAssignee()) return RedirectToAction("Login", "Auth");

            int uid = GetUserId();
            var currentUser = _context.Users.FirstOrDefault(u => u.UserId == uid);
            ViewBag.Username = currentUser?.FullName ?? "Assignee";

            var tasks = _context.Requests
                .Include(r => r.User)
                .Include(r => r.Facility)
                .Where(r => r.AssignedToUserId == uid)
                .ToList();

            ViewBag.TotalTasks = _context.Requests.Count(r => r.AssignedToUserId == uid);
            ViewBag.PendingTasks = _context.Requests.Count(r => r.AssignedToUserId == uid && r.Status == RequestStatus.Assigned);
            ViewBag.InProgressTasks = _context.Requests.Count(r => r.AssignedToUserId == uid && r.Status == RequestStatus.InProgress);
            ViewBag.ResolvedTasks = _context.Requests.Count(r => r.AssignedToUserId == uid && r.Status == RequestStatus.Closed);

            return View(tasks);
        }


        // 🛠️ GET Update Form
        public IActionResult UpdateRequest(int id)
        {
            if (!IsAssignee()) return RedirectToAction("Login", "Auth");

            var req = _context.Requests.FirstOrDefault(r => r.RequestId == id);
            return req == null ? NotFound() : View(req);
        }

        // 🛠️ POST Update Request
        [HttpPost]
        public IActionResult UpdateStatus(int requestId, RequestStatus status)
        {
            if (!IsAssignee()) return RedirectToAction("Login", "Auth");

            var req = _context.Requests.FirstOrDefault(r => r.RequestId == requestId);
            if (req != null)
            {
                req.Status = status;
                req.UpdatedAt = DateTime.Now;

                _context.StatusHistories.Add(new StatusHistory
                {
                    RequestId = req.RequestId,
                    UpdatedByUserId = GetUserId(),
                    Status = status,
                    UpdatedAt = DateTime.Now
                });

                _context.SaveChanges();
            }

            return RedirectToAction("MyTasks");
        }
        public IActionResult RecentTasks()
        {
            if (!IsAssignee()) return RedirectToAction("Login", "Auth");

            int uid = GetUserId();
            var currentUser = _context.Users.FirstOrDefault(u => u.UserId == uid);
            ViewBag.Username = currentUser?.FullName ?? "Assignee";

            var recentTasks = _context.Requests
                .Include(r => r.User)
                .Include(r => r.Facility)
                .Where(r => r.AssignedToUserId == uid)
                .OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt)
                .Take(10)
                .ToList();

            ViewBag.TotalTasks = _context.Requests.Count(r => r.AssignedToUserId == uid);
            ViewBag.PendingTasks = _context.Requests.Count(r => r.AssignedToUserId == uid && r.Status == RequestStatus.Assigned);
            ViewBag.InProgressTasks = _context.Requests.Count(r => r.AssignedToUserId == uid && r.Status == RequestStatus.InProgress);
            ViewBag.ResolvedTasks = _context.Requests.Count(r => r.AssignedToUserId == uid && r.Status == RequestStatus.Closed);

            return View(recentTasks);
        }

        // GET: /Assignee/Profile
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

        // POST: /Assignee/ChangePassword
        [HttpPost]
        public IActionResult ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found!";
                return RedirectToAction("Profile");
            }

            if (user.PasswordHash != currentPassword) // ❗ In production, always use hashed password check
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
