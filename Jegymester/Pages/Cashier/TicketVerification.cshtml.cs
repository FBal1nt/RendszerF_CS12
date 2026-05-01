using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;

namespace JegyMester.Pages.Cashier
{
    [Authorize(Roles = "Cashier")]   // 🔥 Kötelező backend védelem
    public class VerificationModel : PageModel
    {
        private readonly JegyMesterDbContext _context;
        private readonly ILogger<VerificationModel> _logger;

        public VerificationModel(JegyMesterDbContext context, ILogger<VerificationModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Ticket Ticket { get; set; } = default!;

        [TempData]
        public string? StatusMessage { get; set; }

        [BindProperty]
        public int BevittSzam { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Üres placeholder ticket, hogy a Razor ne dobjon null reference hibát
            Ticket = new Ticket
            {
                Id = 0,
                Price = 0,
                Row = 0,
                SeatNumber = 0,
                ScreeningId = 0,
                Screening = await _context.Screenings.FirstOrDefaultAsync(),
                TicketPurchase = await _context.TicketPurchases.FirstOrDefaultAsync(),
                TicketPurchaseId = 0,
                Type = DataContext.Enums.TicketType.Child,
                Valid = false
            };

            return Page();
        }

        // Jegy érvényesítése (használatba vétel)
        public async Task<IActionResult> OnPostVerifyAsync(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.Screening)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket is null)
            {
                StatusMessage = "Ticket not found.";
                return RedirectToPage(new { id });
            }

            if (!ticket.Valid)
            {
                StatusMessage = "Ticket is already used or invalid.";
                return RedirectToPage(new { id = ticket.Id });
            }

            if (ticket.Screening is not null)
            {
                var now = DateTime.Now;

                if (now < ticket.Screening.StartTime.AddMinutes(-10) ||
                    now > ticket.Screening.EndTime.AddMinutes(15))
                {
                    StatusMessage = "Ticket is not valid for this screening time.";
                    return RedirectToPage(new { id = ticket.Id });
                }
            }

            // Atomic update
            var rows = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE Tickets SET Valid = 0 WHERE Id = {ticket.Id} AND Valid = 1");

            if (rows == 0)
            {
                _logger.LogWarning("Concurrent verification or already used ticket {TicketId} by {User}", ticket.Id, User?.Identity?.Name);
                StatusMessage = "Ticket could not be verified (already used or concurrent attempt).";
                return RedirectToPage(new { id = ticket.Id });
            }

            _logger.LogInformation("Ticket {TicketId} verified by {User}", ticket.Id, User?.Identity?.Name);
            StatusMessage = "Ticket verified and marked as used.";
            return RedirectToPage(new { id = ticket.Id });
        }

        // AJAX jegykeresés (QR / ID alapján)
        public async Task<JsonResult> OnPostScanAsync([FromForm] string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return new JsonResult(new { ok = false, message = "Empty code" });

            if (int.TryParse(code, out var id))
            {
                var ticket = await _context.Tickets
                    .Include(t => t.Screening)
                    .Include(t => t.TicketPurchase)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (ticket is null)
                    return new JsonResult(new { ok = false, message = "Ticket not found" });

                return new JsonResult(new
                {
                    ok = true,
                    ticketId = ticket.Id,
                    valid = ticket.Valid,
                    screeningStart = ticket.Screening?.StartTime,
                    screeningEnd = ticket.Screening?.EndTime,
                    row = ticket.Row,
                    seat = ticket.SeatNumber,
                    type = ticket.Type.ToString()
                });
            }

            return new JsonResult(new { ok = false, message = "Unsupported code format" });
        }

        public async Task<IActionResult> OnPostKeresAsync()
        {
            Ticket = await _context.Tickets
                .Include(t => t.Screening)
                .Include(t => t.TicketPurchase)
                .FirstOrDefaultAsync(x => x.Id == BevittSzam);

            if (Ticket is null)
            {
                Ticket = new Ticket
                {
                    Id = 0,
                    Price = 0,
                    Row = 0,
                    SeatNumber = 0,
                    ScreeningId = 0,
                    Screening = await _context.Screenings.FirstOrDefaultAsync(),
                    TicketPurchase = await _context.TicketPurchases.FirstOrDefaultAsync(),
                    TicketPurchaseId = 0,
                    Type = DataContext.Enums.TicketType.Child,
                    Valid = false
                };
            }

            return Page();
        }
    }
}
