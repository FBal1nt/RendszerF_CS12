using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;

namespace JegyMester.Pages.Cashier.Sell_ticket
{
    [Authorize(Roles = "Cashier")]   // Cashier backend védelem
    public class IndexModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public IndexModel(JegyMesterDbContext context)
        {
            _context = context;
        }

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
                if (Search.Contains("@"))
                {
                    var user = _context.Users
                        .FirstOrDefault(f => f.Email.Contains(Search));

                    if (user != null)
                        q = q.Where(tp => tp.UserId == user.Id);
                }
                else if (int.TryParse(Search, out var n))
                {
                    q = q.Where(tp => tp.UserId == n || tp.Id == n);
                }
                else
                {
                    q = q.Where(tp =>
                        tp.Tickets.Any(t =>
                            t.Screening != null &&
                            EF.Functions.Like(t.Screening.Film!.Title!, $"%{Search}%")
                        )
                    );
                }
            }

            if (From.HasValue)
                q = q.Where(tp => tp.PurchaseDateTime >= From.Value);

            if (To.HasValue)
                q = q.Where(tp => tp.PurchaseDateTime <= To.Value);

            TicketPurchases = await q
                .OrderByDescending(tp => tp.PurchaseDateTime)
                .Take(200)
                .ToListAsync();
        }

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
