using Microsoft.AspNetCore.Mvc;
using Online_Help_Desk.Attributes;
using Online_Help_Desk.Models;

namespace Online_Help_Desk.Controllers
{
    [AuthorizeRole(RoleEnum.FacilityHead)]
    public class FacilityHeadController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}

