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

            ViewBag.Facilities = _context.Facilities.ToList(); // for dropdown
            return View();
        }

        // 🧠 NEW REQUEST (POST)
        [HttpPost]
        public IActionResult NewRequest(Request model)
        {
            if (!IsEndUser()) return RedirectToAction("Login", "Auth");

            model.UserId = GetUserId();
            model.Status = RequestStatus.Pending;
            model.CreatedAt = DateTime.Now;

            _context.Requests.Add(model);
            _context.SaveChanges();

            return RedirectToAction("TrackRequests");
        }

        // 🧠 TRACK USER REQUESTS
        public IActionResult TrackRequests()
        {
            if (!IsEndUser()) return RedirectToAction("Login", "Auth");

            int uid = GetUserId();
            var requests = _context.Requests
                .Include(r => r.Facility)
                .Where(r => r.UserId == uid)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return View(requests);
        }

        // 🧠 CLOSE OWN REQUEST
        public IActionResult CloseRequest(int id)
        {
            if (!IsEndUser()) return RedirectToAction("Login", "Auth");

            int uid = GetUserId();
            var req = _context.Requests.FirstOrDefault(r => r.RequestId == id && r.UserId == uid);

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
