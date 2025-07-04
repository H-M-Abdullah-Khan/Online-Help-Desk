using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Help_Desk.Models;

namespace Online_Help_Desk.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == RoleEnum.Admin.ToString();
        }

        public IActionResult Dashboard()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var recentRequests = _context.Requests
                .Include(r => r.User)
                .Include(r => r.Facility)
                .Include(r => r.AssignedToUser)
                .OrderByDescending(r => r.CreatedAt)
                .Take(10)
                .ToList();

            ViewBag.RecentRequests = recentRequests;
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.ActiveRequests = _context.Requests.Count(r => r.Status != RequestStatus.Closed);
            ViewBag.PendingApprovals = _context.ApprovalRequests.Count(a => !a.IsApproved && !a.IsRejected);
            ViewBag.ResolvedIssues = _context.Requests.Count(r => r.Status == RequestStatus.Closed);

            return View();
        }

        public IActionResult ManageUsers()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            return View(_context.Users.ToList());
        }

        public IActionResult ManageRequests()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            var requests = _context.Requests
                .Include(r => r.User)
                .Include(r => r.Facility)
                .ToList();
            return View(requests);
        }

        public IActionResult ManageFacilities()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            return View(_context.Facilities.ToList());
        }

        public IActionResult ManageFacilityHead()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            var heads = _context.Users
                .Where(u => u.Role == RoleEnum.FacilityHead)
                .ToList();
            return View(heads);
        }

        public IActionResult Approvals()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            var pending = _context.ApprovalRequests
                .Include(a => a.User)
                .Where(a => !a.IsApproved && !a.IsRejected)
                .ToList();
            return View(pending);
        }

        public IActionResult Approve(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var req = _context.ApprovalRequests
                .Include(a => a.User)
                .FirstOrDefault(a => a.ApprovalRequestId == id);

            if (req != null)
            {
                req.IsApproved = true;
                req.ActionTakenAt = DateTime.Now;
                req.User.IsActive = true;
                _context.SaveChanges();
            }

            return RedirectToAction("Approvals");
        }

        public IActionResult Reject(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var req = _context.ApprovalRequests.FirstOrDefault(a => a.ApprovalRequestId == id);

            if (req != null)
            {
                req.IsRejected = true;
                req.ActionTakenAt = DateTime.Now;
                _context.SaveChanges();
            }

            return RedirectToAction("Approvals");
        }
    }
}

