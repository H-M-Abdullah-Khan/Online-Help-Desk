using Microsoft.AspNetCore.Mvc;
using Online_Help_Desk.Models;

namespace Online_Help_Desk.Controllers
{
    public class AssigneeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AssigneeController(ApplicationDbContext context) => _context = context;

        public IActionResult Dashboard() => View();

        public IActionResult MyTasks()
        {
            int uid = HttpContext.Session.GetInt32("UserId") ?? 0;
            var tasks = _context.Requests.Where(r => r.AssignedToUserId == uid).ToList();
            return View(tasks);
        }

        public IActionResult UpdateRequest(int id)
        {
            var req = _context.Requests.FirstOrDefault(r => r.RequestId == id);
            return View(req);
        }

        [HttpPost]
        public IActionResult UpdateRequest(Request model)
        {
            var req = _context.Requests.FirstOrDefault(r => r.RequestId == model.RequestId);
            if (req != null)
            {
                req.Status = model.Status;
                req.UpdatedAt = DateTime.Now;
                _context.StatusHistories.Add(new StatusHistory
                {
                    RequestId = req.RequestId,
                    UpdatedByUserId = HttpContext.Session.GetInt32("UserId") ?? 0,
                    Status = model.Status,
                    UpdatedAt = DateTime.Now
                });
                _context.SaveChanges();
            }
            return RedirectToAction("MyTasks");
        }
    }
}


