using Microsoft.AspNetCore.Mvc;

namespace JegyMester.Controllers
{
    public class CinemaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
