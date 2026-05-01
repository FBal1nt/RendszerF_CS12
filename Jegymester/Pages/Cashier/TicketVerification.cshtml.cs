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
    [Authorize(Roles = "Cashier")]
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

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.IsInRole("Cashier"))
            {
                return Forbid();
            }

            return Page();
        }
        
        // Atomic verify + mark-used using conditional UPDATE
        public async Task<IActionResult> OnPostVerifyAsync(int? id)
        {
            if (!User.IsInRole("Cashier"))
            {
                return Forbid();
            }
            if (id == null) return NotFound();

            // Load ticket + screening for business validation (time window)
            var ticket = await _context.Tickets
                .Include(t => t.Screening)
                .ThenInclude(t => t.Film)
                .Include(t => t.TicketPurchase)
                .ThenInclude(t => t.User)
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
                var now = DateTime.UtcNow;
                // Adjust allowed window if needed (e.g., allow 10 minutes before start)
                if (now < ticket.Screening.StartTime.AddMinutes(-10) || now > ticket.Screening.EndTime.AddMinutes(15))
                {
                    StatusMessage = "Ticket is not valid for this screening time.";
                    return RedirectToPage(new { id = ticket.Id });
                }
            }

            // Final atomic update: mark as used only if still valid
            var rows = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE Tickets SET Valid = 0 WHERE Id = {ticket.Id} AND Valid = 1");

            if (rows == 0)
            {
                // Another process already used it (race) or no state change was possible
                _logger.LogWarning("Concurrent verification or already used ticket {TicketId} by {User}", ticket.Id, User?.Identity?.Name);
                StatusMessage = "Ticket could not be verified (already used or concurrent attempt).";
                return RedirectToPage(new { id = ticket.Id });
            }

            _logger.LogInformation("Ticket {TicketId} verified by {User}", ticket.Id, User?.Identity?.Name);
            StatusMessage = "Ticket verified and marked as used.";
            return RedirectToPage(new { id = ticket.Id });
        }

        // AJAX endpoint to lookup ticket by scanned code or id
        // Expects form field "code" (barcode or numeric id)
        public async Task<JsonResult> OnPostScanAsync([FromForm] string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return new JsonResult(new { ok = false, message = "Empty code" });

            // Try parse numeric id first
            if (int.TryParse(code, out var id))
            {
                var ticket = await _context.Tickets
                .Include(t => t.Screening)
                .ThenInclude(s => s.Film)
                .Include(t => t.TicketPurchase)
                .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(p => p.Id == id);

                if (ticket is null) return new JsonResult(new { ok = false, message = "Ticket not found" });

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

            // If using barcode strings, replace above logic to search by barcode column.
            return new JsonResult(new { ok = false, message = "Unsupported code format" });
        }

        public async Task<JsonResult> OnPostScanAndVerifyAsync([FromForm] string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return new JsonResult(new { ok = false, message = "Empty code" });

            if (!int.TryParse(code, out var id))
                return new JsonResult(new { ok = false, message = "Invalid format" });

            var ticket = await _context.Tickets
                .Include(t => t.Screening)
                .ThenInclude(s => s.Film)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return new JsonResult(new { ok = false, message = "Ticket not found" });

            if (!ticket.Valid)
                return new JsonResult(new { ok = false, message = "Already used" });

            var now = DateTime.UtcNow;

            bool ignoreTime = true; // EZT FALSE HA NEM DEBUG MÓDBAN VAGY!!!!!!!!!

            if (!ignoreTime && ticket.Screening != null &&
               (now < ticket.Screening.StartTime.AddMinutes(-10) ||
                now > ticket.Screening.EndTime.AddMinutes(15)))
            {
                return new JsonResult(new { ok = false, message = "Too early or too late for this screening" });
            }

            var rows = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE Tickets SET Valid = 0 WHERE Id = {ticket.Id} AND Valid = 1");

            if (rows == 0)
                return new JsonResult(new { ok = false, message = "Already used (race)" });

            return new JsonResult(new
            {
                ok = true,
                message = "Ticket verified",
                ticketId = ticket.Id,
                row = ticket.Row,
                seat = ticket.SeatNumber,
                type = ticket.Type.ToString()
            });
        }
    }
}
