using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JegyMester.Pages.Cashier
{
    [Authorize(Roles = "Cashier")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
