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
    public class DeleteModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public DeleteModel(JegyMesterDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TicketPurchase TicketPurchase { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var purchase = await _context.TicketPurchases
                .Include(tp => tp.Tickets)
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (purchase == null)
                return NotFound();

            TicketPurchase = purchase;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var purchase = await _context.TicketPurchases
                .Include(tp => tp.Tickets)
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (purchase != null)
            {
                // Jegyek törlése először
                _context.Tickets.RemoveRange(purchase.Tickets);

                // Majd a vásárlás törlése
                _context.TicketPurchases.Remove(purchase);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
