using Microsoft.AspNetCore.Mvc;

namespace JegyMester.Controllers
{
    public class RoomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
