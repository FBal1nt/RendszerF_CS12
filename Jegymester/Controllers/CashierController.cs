using Microsoft.AspNetCore.Mvc;

namespace JegyMester.Controllers
{
    public class CashierController : Controller
    {
        public IActionResult Verification()
        {
            return View();
        }
    }
}
