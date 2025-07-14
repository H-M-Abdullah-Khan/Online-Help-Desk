using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Help_Desk.Models;

namespace Online_Help_Desk.Controllers
{
    public class FacilityHeadController : Controller
    {
        private readonly ApplicationDbContext _context;
        public FacilityHeadController(ApplicationDbContext context) => _context = context;

        private bool IsFacilityHead()
        {
            return HttpContext.Session.GetString("Role") == RoleEnum.FacilityHead.ToString();
        }

        private int GetUserId()
        {
            return HttpContext.Session.GetInt32("UserId") ?? 0;
        }

        //  Dashboard
        public IActionResult Dashboard()
        {
            if (!IsFacilityHead()) return RedirectToAction("Login", "Auth");

            int uid = GetUserId();
            var currentUser = _context.Users.FirstOrDefault(u => u.UserId == uid);
            ViewBag.Username = currentUser?.FullName ?? "Facility Head";

            var assignedRequests = _context.Requests
                .Include(r => r.User)
                .Include(r => r.Facility)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            ViewBag.TotalAssigned = assignedRequests.Count;
            ViewBag.PendingCount = assignedRequests.Count(r => r.Status == RequestStatus.Assigned);
            ViewBag.InProgressCount = assignedRequests.Count(r => r.Status == RequestStatus.InProgress);
            ViewBag.ResolvedCount = assignedRequests.Count(r => r.Status == RequestStatus.Closed);

            return View(assignedRequests);
        }


        // Show Pending Requests to Assign
        public IActionResult AssignRequests()
        {
            if (!IsFacilityHead()) return RedirectToAction("Login", "Auth");

            int uid = GetUserId();
            var currentUser = _context.Users.FirstOrDefault(u => u.UserId == uid);
            ViewBag.Username = currentUser?.FullName ?? "Facility Head";

            var pendingRequests = _context.Requests
                .Include(r => r.User)
                .Include(r => r.Facility)
                .Where(r => r.Status == RequestStatus.Pending)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            ViewBag.Assignees = _context.Users
                .Where(u => u.Role == RoleEnum.Assignee && u.IsActive)
                .ToList();

            return View(pendingRequests);
        }

        // Assign to Assignee
        [HttpPost]
        public IActionResult Assign(int id, int userId)
        {
            if (!IsFacilityHead()) return RedirectToAction("Login", "Auth");

            int uid = GetUserId();
            var currentUser = _context.Users.FirstOrDefault(u => u.UserId == uid);
            ViewBag.Username = currentUser?.FullName ?? "Facility Head";

            var req = _context.Requests.FirstOrDefault(r => r.RequestId == id);
            if (req != null)
            {
                req.AssignedToUserId = userId;
                req.Status = RequestStatus.Assigned;
                req.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
            }
            return RedirectToAction("AssignRequests");
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
