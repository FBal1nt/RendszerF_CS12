using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JegyMester.Pages
{
    [Authorize(Roles = "Admin")]   // 🔥 Backend szintű admin védelem
    public class AdminIndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
