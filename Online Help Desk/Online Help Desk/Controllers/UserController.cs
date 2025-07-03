using Microsoft.AspNetCore.Mvc;
using Online_Help_Desk.Models;

namespace Online_Help_Desk.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context) => _context = context;

        public IActionResult Dashboard() => View();

        public IActionResult NewRequest() => View();

        [HttpPost]
        public IActionResult NewRequest(Request model)
        {
            model.UserId = HttpContext.Session.GetInt32("UserId") ?? 0;
            model.Status = RequestStatus.Pending;
            _context.Requests.Add(model);
            _context.SaveChanges();
            return RedirectToAction("TrackRequests");
        }

        public IActionResult TrackRequests()
        {
            int uid = HttpContext.Session.GetInt32("UserId") ?? 0;
            var data = _context.Requests.Where(r => r.UserId == uid).ToList();
            return View(data);
        }

        public IActionResult CloseRequest(int id)
        {
            var req = _context.Requests.FirstOrDefault(r => r.RequestId == id);
            if (req != null && req.Status != RequestStatus.Closed)
            {
                req.Status = RequestStatus.Closed;
                req.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
            }
            return RedirectToAction("TrackRequests");
        }
    }
}

