using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JegyMester.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // JWT token törlése
            HttpContext.Response.Cookies.Delete("auth_token");

            return RedirectToPage("/Login");
        }
    }
}
