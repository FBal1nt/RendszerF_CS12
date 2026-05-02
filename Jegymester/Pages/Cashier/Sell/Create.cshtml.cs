using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;
using JegyMester.DataContext.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JegyMester.Pages.Cashier.Sell_ticket
{
    [Authorize(Roles = "Cashier")]
    public class CreateModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public CreateModel(JegyMesterDbContext context)
        {
            _context = context;
        }

        // Simple DTO for the tickets being sold in one purchase
        public class SaleItem
        {
            public int ScreeningId { get; set; }
            public TicketType Type { get; set; }
            public int Row { get; set; }
            public int SeatNumber { get; set; }
        }

        [BindProperty]
        public List<SaleItem> Items { get; set; } = new();

        // Optional: if you still want to expose a TicketPurchase model for additional fields
        [BindProperty]
        public TicketPurchase ticketPurchase { get; set; } = new();

        public IList<DataContext.Entities.Screening> AvailableScreenings { get; set; } = Array.Empty<DataContext.Entities.Screening>();
        public IList<Ticket> AvailableTickets { get; set; } = Array.Empty<Ticket>();

        public SelectList TicketTypeOptions { get; set; } = default!;

        public async Task OnGetAsync()
        {
            AvailableScreenings = await _context.Screenings
                .Include(s => s.Film)
                .OrderBy(s => s.StartTime)
                .Take(50)
                .ToListAsync();

            Console.WriteLine($"Items count: {Items.Count}");

            TicketTypeOptions = new SelectList(Enum.GetValues<TicketType>().Select(t => new { Id = (int)t, Name = t.ToString() }), "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine($"===============================================Items count POST: {Items?.Count}=================================================================");
            if (Items == null || !Items.Any())
            {
                ModelState.AddModelError(string.Empty, "No tickets added.");
                await OnGetAsync();
                return Page();
            }

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            // Prevent duplicate seat selection within this submission
            var dup = Items.GroupBy(i => (i.ScreeningId, i.Row, i.SeatNumber)).Any(g => g.Count() > 1);
            if (dup)
            {
                ModelState.AddModelError(string.Empty, "Duplicate seat in submission.");
                await OnGetAsync();
                return Page();
            }

            // Check for existing tickets (atomicity ensured by DB transaction + unique index)
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // Build a set of screening/row/seat tuples to check
                var existingTickets = await _context.Tickets
                .Where(t => t.ScreeningId == Items.First().ScreeningId)
                .ToListAsync();

                var conflicts = existingTickets.Any(t =>
                    Items.Any(i =>
                         i.Row == t.Row &&
                         i.SeatNumber == t.SeatNumber
                    ));

                if (conflicts)
                {
                    ModelState.AddModelError(string.Empty, "One or more seats are already taken.");
                    await tx.RollbackAsync();
                    await OnGetAsync();
                    return Page();
                }

                // Create TicketPurchase
                var purchase = new TicketPurchase
                {
                    PurchaseDateTime = DateTime.UtcNow,
                    UserId = _context.Users.Where(t => t.Name.Equals("Pénztár")).Select(u => u.Id).FirstOrDefault() // provided by cashier or 0; adjust to use authenticated user if desired
                };
                _context.TicketPurchases.Add(purchase);
                await _context.SaveChangesAsync();

                // Create Tickets
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

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var msg = ex.ToString();
                    throw new Exception(msg);
                }
                await tx.CommitAsync();

                return RedirectToPage("./Details", new { id = purchase.Id });
            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Failed to complete sale. Please try again.");
                await OnGetAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnGetRefreshListAsync(int screenId)
        {
            // Szűrés a kapott azonosító alapján a DataContext segítségével
            var filteredItems = await _context.Tickets
                .Where(x => x.ScreeningId == screenId)
                .AsNoTracking()
                .ToListAsync();

            // A szűrt listát adjuk át a Partial View-nak
            return Page();
        }

        private int CalculatePrice(TicketType type)
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

        public async Task<JsonResult> OnGetTakenSeatsAsync(int screeningId)
        {
            var taken = await _context.Tickets
                .Where(t => t.ScreeningId == screeningId)
                .Select(t => new { t.Row, t.SeatNumber })
                .ToListAsync();

            return new JsonResult(taken);
        }
    }
}
