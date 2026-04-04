using Microsoft.AspNetCore.Mvc;

namespace JegyMester.Controllers
{
    public class TicketPurchaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
