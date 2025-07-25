﻿using Microsoft.AspNetCore.Mvc;
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
        // Save Admin Role
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == RoleEnum.Admin.ToString();
        }
        // Dashboard
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
        // ManageUsers
        public IActionResult ManageUsers()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.AdminCount = _context.Users.Count(u => u.Role == RoleEnum.Admin);
            ViewBag.FacilityHeadCount = _context.Users.Count(u => u.Role == RoleEnum.FacilityHead);
            ViewBag.AssigneeCount = _context.Users.Count(u => u.Role == RoleEnum.Assignee);


            return View(_context.Users.ToList());
        }
        // ManageRequests
        public IActionResult ManageRequests()
        {
            var requests = _context.Requests
               .Include(r => r.User)
               .Include(r => r.Facility)
               .Include(r => r.AssignedToUser)
               .ToList();

            ViewBag.TotalRequests = requests.Count;
            ViewBag.PendingRequests = requests.Count(r => r.Status == RequestStatus.Pending);
            ViewBag.InProgressRequests = requests.Count(r => r.Status == RequestStatus.InProgress);
            ViewBag.ClosedRequests = requests.Count(r => r.Status == RequestStatus.Closed);

            return View(requests);
        }

        // ManageFacilities
        public IActionResult ManageFacilities()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            ViewBag.TotalFacilities = _context.Facilities.Count();
            ViewBag.FacilityRequests = _context.Requests
                .Select(r => r.FacilityId)
                .Distinct()
                .Count();

            ViewBag.UniqueLocations = _context.Facilities
                .Select(f => f.Location)
                .Distinct()
                .Count();

            ViewBag.UnusedFacilities = _context.Facilities
                .Count(f => !_context.Requests.Any(r => r.FacilityId == f.FacilityId));

            return View(_context.Facilities.ToList());
        }

        // AddFacility (Post)

        [HttpPost]
        public IActionResult AddFacility(Facility facility)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(facility.Name))
            {
                _context.Facilities.Add(facility);
                _context.SaveChanges();
                TempData["Success"] = "Facility added successfully!";
            }
            else
            {
                TempData["Error"] = "Facility name is required.";
            }

            return RedirectToAction("ManageFacilities");
        }

        // ManageFacilityHead
        public IActionResult ManageFacilityHead()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            ViewBag.TotalFH = _context.Users.Count(u => u.Role == RoleEnum.FacilityHead);
            ViewBag.ApprovedFH = _context.Users.Count(u => u.Role == RoleEnum.FacilityHead && u.IsActive);
            ViewBag.PendingFH = _context.ApprovalRequests.Count(r => r.RequestedRole == RoleEnum.FacilityHead && !r.IsApproved && !r.IsRejected);
            ViewBag.FacilityCount = _context.Facilities.Count();

            return View(_context.Users.Where(u => u.Role == RoleEnum.FacilityHead).ToList());
        }

        // Approvals
        public IActionResult Approvals()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var pending = _context.ApprovalRequests
                .Include(a => a.User)
                .Where(a => !a.IsApproved && !a.IsRejected)
                .ToList();

            ViewBag.TotalRequests = _context.ApprovalRequests.Count();
            ViewBag.PendingApprovals = _context.ApprovalRequests.Count(a => !a.IsApproved && !a.IsRejected);
            ViewBag.ApprovedRequests = _context.ApprovalRequests.Count(a => a.IsApproved);
            ViewBag.RejectedRequests = _context.ApprovalRequests.Count(a => a.IsRejected);

            return View(pending);
        }

        // Approve
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

            TempData["Approved"] = "User has been approved successfully!";
            return RedirectToAction("Approvals");

        }

        // Reject
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

            TempData["Rejected"] = "User request has been rejected.";
            return RedirectToAction("Approvals");

        }
        // GET: Profile
        public IActionResult Profile()
        {
            int uid = HttpContext.Session.GetInt32("UserId") ?? 0;
            var user = _context.Users.FirstOrDefault(u => u.UserId == uid);
            if (user == null) return RedirectToAction("Login", "Auth");
            return View(user);
        }

        // POST: UpdateProfile
        [HttpPost]
        public IActionResult UpdateProfile(User updatedUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == updatedUser.UserId);
            if (user != null)
            {
                user.FullName = updatedUser.FullName;
                user.Email = updatedUser.Email;
                user.Username = updatedUser.Username;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }
            return RedirectToAction("Profile");
        }

        // POST: ChangePassword
        [HttpPost]
        public IActionResult ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null || user.PasswordHash != currentPassword)
            {
                TempData["ErrorMessage"] = "Current password is incorrect.";
                return RedirectToAction("Profile");
            }

            user.PasswordHash = newPassword;
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("Profile");
        }

    }
}

