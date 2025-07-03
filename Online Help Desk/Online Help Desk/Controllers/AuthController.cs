using Microsoft.AspNetCore.Mvc;
using Online_Help_Desk.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore; // Required for Entity Framework Core operations

namespace Online_Help_Desk.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AuthController(ApplicationDbContext context) => _context = context;

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password && u.IsActive);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Role", user.Role.ToString());

                return user.Role switch
                {
                    RoleEnum.Admin => RedirectToAction("Dashboard", "Admin"),
                    RoleEnum.EndUser => RedirectToAction("Dashboard", "User"),
                    RoleEnum.FacilityHead => RedirectToAction("Dashboard", "FacilityHead"),
                    RoleEnum.Assignee => RedirectToAction("Dashboard", "Assignee"),
                    _ => RedirectToAction("Login")
                };
            }
            ViewBag.Error = "Invalid credentials or not approved.";
            return View();
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(Users model, RoleEnum requestedRole, string reason)
        {
            model.IsActive = requestedRole == RoleEnum.EndUser;
            _context.Users.Add(model);
            _context.SaveChanges();

            if (!model.IsActive)
            {
                _context.ApprovalRequests.Add(new ApprovalRequest
                {
                    UserId = model.UserId,
                    RequestedRole = requestedRole,
                    Reason = reason
                });
                _context.SaveChanges();
            }

            TempData["Success"] = "Registered. Please wait for admin approval.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}


