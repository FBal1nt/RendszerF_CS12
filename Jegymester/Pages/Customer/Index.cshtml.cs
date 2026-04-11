using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JegyMester.Pages.Customer
{
    [Authorize(Roles = "Customer")]
    public class IndexModelCustomer : PageModel
    {
        public void OnGet()
        {
        }
    }
}
