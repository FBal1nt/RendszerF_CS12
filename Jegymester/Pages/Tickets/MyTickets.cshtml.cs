using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JegyMester.Pages.Tickets
{
    [Authorize] // 🔥 csak bejelentkezett felhasználó érheti el
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
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            Purchases = await _context.TicketPurchases
                .Where(tp => tp.UserId == userId)
                .Include(tp => tp.Tickets)
                    .ThenInclude(t => t.Screening)
                        .ThenInclude(s => s.Film)
                .Include(tp => tp.Tickets)
                    .ThenInclude(t => t.Screening)
                        .ThenInclude(s => s.Room)
                            .ThenInclude(r => r.Cinema)
                .OrderByDescending(tp => tp.PurchaseDateTime)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int ticketId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var ticket = await _context.Tickets
                .Include(t => t.Screening)
                .Include(t => t.TicketPurchase)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                TempData["Error"] = "A jegy nem található.";
                return RedirectToPage();
            }

            if (ticket.TicketPurchase.UserId != userId)
            {
                TempData["Error"] = "Nincs jogosultságod a jegy törléséhez.";
                return RedirectToPage();
            }

            if (DateTime.Now > ticket.Screening.StartTime.AddHours(-4))
            {
                TempData["Error"] = "A jegy már nem törölhető (kevesebb mint 4 óra van hátra).";
                return RedirectToPage();
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

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
