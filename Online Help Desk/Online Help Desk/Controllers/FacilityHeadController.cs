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

        // 📊 Dashboard (secure)
        public IActionResult Dashboard()
        {
            if (!IsFacilityHead()) return RedirectToAction("Login", "Auth");

            int uid = GetUserId();
            var currentUser = _context.Users.FirstOrDefault(u => u.UserId == uid);
            ViewBag.Username = currentUser?.FullName ?? "Facility Head";
            var assignedRequests = _context.Requests
                .Include(r => r.User)
                .Include(r => r.Facility)
                .Where(r => r.AssignedToUserId == uid)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            ViewBag.TotalAssigned = assignedRequests.Count;
            ViewBag.PendingCount = assignedRequests.Count(r => r.Status == RequestStatus.Assigned);
            ViewBag.InProgressCount = assignedRequests.Count(r => r.Status == RequestStatus.InProgress);
            ViewBag.ResolvedCount = assignedRequests.Count(r => r.Status == RequestStatus.Closed);

            return View(assignedRequests);
        }

        // 🧾 Show Pending Requests to Assign
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

        // ✅ Assign to Assignee
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

    }
}
