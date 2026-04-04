using Microsoft.AspNetCore.Mvc;

namespace JegyMester.Controllers
{
    public class ScreeningController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
