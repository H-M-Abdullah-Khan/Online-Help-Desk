using Microsoft.AspNetCore.Mvc;

namespace Online_Help_Desk.Controllers
{
    public class RequestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
