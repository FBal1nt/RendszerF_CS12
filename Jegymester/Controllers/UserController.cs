using Microsoft.AspNetCore.Mvc;

namespace JegyMester.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
