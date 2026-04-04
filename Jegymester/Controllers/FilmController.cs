using Microsoft.AspNetCore.Mvc;

namespace JegyMester.Controllers
{
    public class FilmController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
