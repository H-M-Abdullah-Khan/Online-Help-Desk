using Microsoft.AspNetCore.Mvc;

namespace Online_Help_Desk.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
