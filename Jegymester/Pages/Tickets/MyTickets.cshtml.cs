using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> OnGetAsync()
        {
            // A bejelentkezett felhasználó ID-ja
            var userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
            {
                return RedirectToPage("/Login");
            }
            int userId = int.Parse(userIdString);

            Purchases = await _context.TicketPurchases
            .Include(tp => tp.Tickets)
                .ThenInclude(t => t.Screening)
                    .ThenInclude(s => s.Film)
            .Include(tp => tp.Tickets)
                .ThenInclude(t => t.Screening.Room)
            .Where(tp =>
                tp.UserId == userId &&
                tp.Tickets.Any(t =>
                    t.Valid == true &&
                    t.Screening != null &&
                    t.Screening.Film != null &&
                    t.Screening.Room != null
                )
            )
            .OrderByDescending(tp => tp.PurchaseDateTime)
            .ToListAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int ticketId)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
                return RedirectToPage("/Login");

            int userId = int.Parse(userIdString);

            // Jegy betöltése
            var ticket = await _context.Tickets
                .Include(t => t.Screening)
                .Include(t => t.TicketPurchase)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                TempData["Error"] = "A jegy nem található.";
                return RedirectToPage();
            }

            // Csak a saját jegy törölhető
            if (ticket.TicketPurchase.UserId != userId)
            {
                TempData["Error"] = "Nincs jogosultságod a jegy törléséhez.";
                return RedirectToPage();
            }

            // 4 órás szabály
            if (DateTime.Now > ticket.Screening.StartTime.AddHours(-4))
            {
                TempData["Error"] = "A jegy már nem törölhető (kevesebb mint 4 óra van hátra).";
                return RedirectToPage();
            }

            // Jegy törlése
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            // Ha a vásárlásban már nincs jegy → töröljük a TicketPurchase-t is
            var remaining = await _context.Tickets
                .CountAsync(t => t.TicketPurchaseId == ticket.TicketPurchaseId);

            if (remaining == 0)
            {
                _context.TicketPurchases.Remove(ticket.TicketPurchase);
                await _context.SaveChangesAsync();
            }

            TempData["Message"] = "A jegy sikeresen törölve lett.";
            return RedirectToPage();
        }

    }
}
