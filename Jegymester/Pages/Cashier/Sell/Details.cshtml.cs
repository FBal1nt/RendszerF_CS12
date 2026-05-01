using System;
using System.Collections.Generic;
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
    public class DetailsModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public DetailsModel(JegyMesterDbContext context)
        {
            _context = context;
        }

        public TicketPurchase TicketPurchase { get; set; } = default!;

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var purchase = await _context.TicketPurchases
                .Include(tp => tp.Tickets)
                    .ThenInclude(t => t.Screening)
                        .ThenInclude(s => s.Film)
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (purchase == null)
                return NotFound();

            TicketPurchase = purchase;
            return Page();
        }

        // Jegy érvényességének kapcsolása (Cashier funkció)
        public async Task<IActionResult> OnPostToggleTicketValidAsync(int ticketId, int purchaseId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                StatusMessage = "Ticket not found.";
                return RedirectToPage(new { id = purchaseId });
            }

            ticket.Valid = !ticket.Valid;
            await _context.SaveChangesAsync();

            StatusMessage = ticket.Valid ? "Ticket revalidated." : "Ticket marked as used.";
            return RedirectToPage(new { id = purchaseId });
        }

        // Jegy törlése a vásárlásból (Cashier funkció)
        public async Task<IActionResult> OnPostRemoveTicketAsync(int ticketId, int purchaseId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                StatusMessage = "Ticket not found.";
                return RedirectToPage(new { id = purchaseId });
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            StatusMessage = "Ticket removed from purchase.";
            return RedirectToPage(new { id = purchaseId });
        }
    }
}
