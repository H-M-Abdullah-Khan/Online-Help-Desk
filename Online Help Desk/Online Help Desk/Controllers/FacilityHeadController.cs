using Microsoft.AspNetCore.Mvc;
using Online_Help_Desk.Models;
using Online_Help_Desk.Models.OnlineHelpDesk.Models;

namespace Online_Help_Desk.Controllers
{
    public class FacilityHeadController : Controller
    {
        private readonly ApplicationDbContext _context;
        public FacilityHeadController(ApplicationDbContext context) => _context = context;

        public IActionResult Dashboard() => View();

        public IActionResult AssignRequests()
        {
            var data = _context.Requests.Where(r => r.Status == RequestStatus.Pending).ToList();
            return View(data);
        }

        public IActionResult Assign(int id, int userId)
        {
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

        public IActionResult Reports() => View();
    }
}

