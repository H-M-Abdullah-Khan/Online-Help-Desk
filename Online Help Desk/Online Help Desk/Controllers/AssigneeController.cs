using Microsoft.AspNetCore.Mvc;
using Online_Help_Desk.Attributes;
using Online_Help_Desk.Models;

namespace Online_Help_Desk.Controllers
{
    [AuthorizeRole(RoleEnum.Assignee)]
    public class AssigneeController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}

