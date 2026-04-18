using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;

namespace JegyMester.Pages.Cashier.Sell_ticket
{
    public class IndexModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public IndexModel(JegyMesterDbContext context)
        {
            _context = context;
        }

        // Filters
        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? From { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? To { get; set; }

        public IList<TicketPurchase> TicketPurchases { get; set; } = default!;

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            var q = _context.TicketPurchases
                .Include(tp => tp.Tickets)
                    .ThenInclude(t => t.Screening)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                // allow searching by user id or purchase id
                if (int.TryParse(Search, out var n))
                {
                    q = q.Where(tp => tp.UserId == n || tp.Id == n);
                }
                else
                {
                    // fallback: search in related film title (if loaded)
                    q = q.Where(tp => tp.Tickets.Any(t => t.Screening != null && EF.Functions.Like(t.Screening.Film!.Title!, $"%{Search}%")));
                }
            }

            if (From.HasValue)
            {
                q = q.Where(tp => tp.PurchaseDateTime >= From.Value);
            }

            if (To.HasValue)
            {
                q = q.Where(tp => tp.PurchaseDateTime <= To.Value);
            }

            TicketPurchases = await q
                .OrderByDescending(tp => tp.PurchaseDateTime)
                .Take(200) // limit to recent 200 for performance
                .ToListAsync();
        }

        // Revert (cancel) an entire purchase: delete tickets then purchase in a transaction
        public async Task<IActionResult> OnPostRevertAsync(int id)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var purchase = await _context.TicketPurchases
                    .Include(tp => tp.Tickets)
                    .FirstOrDefaultAsync(tp => tp.Id == id);

                if (purchase == null)
                {
                    StatusMessage = "Purchase not found.";
                    return RedirectToPage();
                }

                // remove tickets (frees seats)
                _context.Tickets.RemoveRange(purchase.Tickets);
                _context.TicketPurchases.Remove(purchase);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                StatusMessage = $"Purchase #{id} reverted.";
                return RedirectToPage();
            }
            catch (DbUpdateException)
            {
                await tx.RollbackAsync();
                StatusMessage = "Could not revert purchase. Try again.";
                return RedirectToPage();
            }
        }
    }
}
