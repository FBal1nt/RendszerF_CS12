using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;
using JegyMester.DataContext.Enums;

namespace JegyMester.Pages.Cashier.Sell_ticket
{
    [Authorize(Roles = "Cashier")]   // Cashier backend védelem
    public class CreateModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public CreateModel(JegyMesterDbContext context)
        {
            _context = context;
        }

        public class SaleItem
        {
            public int ScreeningId { get; set; }
            public TicketType Type { get; set; }
            public int Row { get; set; }
            public int SeatNumber { get; set; }
        }

        [BindProperty]
        public List<SaleItem> Items { get; set; } = new();

        [BindProperty]
        public TicketPurchase TicketPurchase { get; set; } = new();

        public IList<DataContext.Entities.Screening> AvailableScreenings { get; set; } = Array.Empty<DataContext.Entities.Screening>();

        public SelectList TicketTypeOptions { get; set; } = default!;

        public async Task OnGetAsync()
        {
            AvailableScreenings = await _context.Screenings
                .Include(s => s.Film)
                .OrderBy(s => s.StartTime)
                .Take(50)
                .ToListAsync();

            TicketTypeOptions = new SelectList(
                Enum.GetValues<TicketType>().Select(t => new { Id = (int)t, Name = t.ToString() }),
                "Id",
                "Name"
            );
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Items == null || Items.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Add at least one ticket.");
            }

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var dup = Items.GroupBy(i => (i.ScreeningId, i.Row, i.SeatNumber))
                           .Any(g => g.Count() > 1);

            if (dup)
            {
                ModelState.AddModelError(string.Empty, "Duplicate seat in submission.");
                await OnGetAsync();
                return Page();
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var screeningIds = Items.Select(i => i.ScreeningId).Distinct().ToArray();

                var conflicts = await _context.Tickets
                    .Where(t => screeningIds.Contains(t.ScreeningId) &&
                                Items.Any(i => i.ScreeningId == t.ScreeningId &&
                                               i.Row == t.Row &&
                                               i.SeatNumber == t.SeatNumber))
                    .ToListAsync();

                if (conflicts.Any())
                {
                    ModelState.AddModelError(string.Empty, "One or more seats are already taken.");
                    await tx.RollbackAsync();
                    await OnGetAsync();
                    return Page();
                }

                var purchase = new TicketPurchase
                {
                    PurchaseDateTime = DateTime.UtcNow,
                    UserId = TicketPurchase.UserId
                };

                _context.TicketPurchases.Add(purchase);
                await _context.SaveChangesAsync();

                foreach (var item in Items)
                {
                    var ticket = new Ticket
                    {
                        ScreeningId = item.ScreeningId,
                        Type = item.Type,
                        Row = item.Row,
                        SeatNumber = item.SeatNumber,
                        Valid = true,
                        Price = CalculatePrice(item.Type),
                        TicketPurchaseId = purchase.Id
                    };

                    _context.Tickets.Add(ticket);
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return RedirectToPage("./Details", new { id = purchase.Id });
            }
            catch (DbUpdateException)
            {
                await tx.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Failed to complete sale. Please try again.");
                await OnGetAsync();
                return Page();
            }
        }

        private decimal CalculatePrice(TicketType type)
        {
            return type switch
            {
                TicketType.Adult => 2000,
                TicketType.Student => 1500,
                TicketType.Child => 1200,
                TicketType.Senior => 1500,
                TicketType.VIP => 3500,
                _ => 2000
            };
        }

    }
}
