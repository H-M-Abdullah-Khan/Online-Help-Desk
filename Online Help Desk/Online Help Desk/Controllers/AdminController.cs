using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Help_Desk.Models;

namespace Online_Help_Desk.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context) => _context = context;

        public IActionResult Dashboard() => View();

        public IActionResult ManageUsers() => View(_context.Users.ToList());

        public IActionResult ManageRequests() => View(_context.Requests.Include(r => r.User).Include(r => r.Facility).ToList());

        public IActionResult ManageFacilities() => View(_context.Facilities.ToList());

        public IActionResult ManageFacilityHead() => View(_context.Users.Where(u => u.Role == RoleEnum.FacilityHead).ToList());

        public IActionResult Approvals() => View(_context.ApprovalRequests.Include(a => a.User).Where(a => !a.IsApproved && !a.IsRejected).ToList());

        public IActionResult Approve(int id)
        {
            var req = _context.ApprovalRequests.Include(a => a.User).FirstOrDefault(a => a.ApprovalRequestId == id);
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

