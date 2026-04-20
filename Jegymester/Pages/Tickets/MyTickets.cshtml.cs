using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;

namespace JegyMester.Pages.Tickets
{
    public class MyTicketsModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public MyTicketsModel(JegyMesterDbContext context)
        {
            _context = context;
        }

        public IList<TicketPurchase> Purchases { get; set; } = new List<TicketPurchase>();

        public async Task OnGetAsync()
        {
            // A bejelentkezett felhasználó ID-ja
            var userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
                return;

            int userId = int.Parse(userIdString);

            Purchases = await _context.TicketPurchases
                .Include(tp => tp.Tickets)
                    .ThenInclude(t => t.Screening)
                        .ThenInclude(s => s.Film)
                .Include(tp => tp.Tickets)
                    .ThenInclude(t => t.Screening.Room)
                .Where(tp => tp.UserId == userId)
                .OrderByDescending(tp => tp.PurchaseDateTime)
                .ToListAsync();
        }
    }
}
