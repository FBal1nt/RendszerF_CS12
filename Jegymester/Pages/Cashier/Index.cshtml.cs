using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace JegyMester.Pages.Cashier
{
    [Authorize(Roles = "Cashier")]   // Cashier backend védelem
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
