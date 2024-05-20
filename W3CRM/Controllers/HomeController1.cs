using Microsoft.AspNetCore.Mvc;

namespace Vitaly_Manager.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
