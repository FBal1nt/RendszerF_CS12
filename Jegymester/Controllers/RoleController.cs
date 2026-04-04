using Microsoft.AspNetCore.Mvc;

namespace JegyMester.Controllers
{
    public class RoleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
