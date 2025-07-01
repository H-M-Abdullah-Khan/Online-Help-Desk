using Microsoft.AspNetCore.Mvc;
using Online_Help_Desk.Attributes;
using Online_Help_Desk.Models;

namespace Online_Help_Desk.Controllers
{
    [AuthorizeRole(RoleEnum.EndUser)]
    public class UserController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}

