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

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Username and password are required.";
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || !VerifyPasswordHash(password, user.PasswordHash))
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetInt32("UserRole", (int)user.Role);
            HttpContext.Session.SetString("UserName", user.Username);

            return RedirectToDashboard(user.Role);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Users newUser)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Username == newUser.Username || u.Email == newUser.Email))
                {
                    ViewBag.ErrorMessage = "Username or Email already exists.";
                    return View(newUser);
                }

                newUser.PasswordHash = HashPassword(newUser.PasswordHash);
                newUser.DateCreated = DateTime.Now;
                newUser.IsActive = true; // Default to active
                newUser.Role = RoleEnum.EndUser; // Default role for new registrations

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                ViewBag.SuccessMessage = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }
            return View(newUser);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            string hashOfInput = HashPassword(password);
            return hashOfInput == storedHash;
        }

        private IActionResult RedirectToDashboard(RoleEnum role)
        {
            return role switch
            {
                RoleEnum.Admin => RedirectToAction("Dashboard", "Admin"),
                RoleEnum.EndUser => RedirectToAction("Dashboard", "User"),
                RoleEnum.FacilityHead => RedirectToAction("Dashboard", "FacilityHead"),
                RoleEnum.Assignee => RedirectToAction("Dashboard", "Assignee"),
                _ => RedirectToAction("Login", "Auth"),
            };
        }
    }
}

