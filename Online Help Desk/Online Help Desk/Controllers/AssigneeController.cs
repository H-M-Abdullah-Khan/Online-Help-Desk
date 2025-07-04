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

        // 📊 Dashboard for Assignee
        public IActionResult Dashboard()
        {
            if (!IsAssignee()) return RedirectToAction("Login", "Auth");

            int uid = GetUserId();
            var assigned = _context.Requests.Where(r => r.AssignedToUserId == uid);

            ViewBag.TotalTasks = assigned.Count();
            ViewBag.InProgress = assigned.Count(r => r.Status == RequestStatus.InProgress);
            ViewBag.Pending = assigned.Count(r => r.Status == RequestStatus.Assigned);
            ViewBag.Completed = assigned.Count(r => r.Status == RequestStatus.Closed);

            return View(assigned.ToList()); // optional: show recent tasks
        }

        // 📋 View My Assigned Tasks
        public IActionResult MyTasks()
        {
            if (!IsAssignee()) return RedirectToAction("Login", "Auth");

            int uid = GetUserId();
            var tasks = _context.Requests
                .Include(r => r.User)
                .Include(r => r.Facility)
                .Where(r => r.AssignedToUserId == uid)
                .ToList();

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
        public IActionResult UpdateRequest(Request model)
        {
            if (!IsAssignee()) return RedirectToAction("Login", "Auth");

            var req = _context.Requests.FirstOrDefault(r => r.RequestId == model.RequestId);
            if (req != null)
            {
                req.Status = model.Status;
                req.UpdatedAt = DateTime.Now;

                _context.StatusHistories.Add(new StatusHistory
                {
                    RequestId = req.RequestId,
                    UpdatedByUserId = GetUserId(),
                    Status = model.Status,
                    UpdatedAt = DateTime.Now
                });

                _context.SaveChanges();
            }

            return RedirectToAction("MyTasks");
        }
    }
}
