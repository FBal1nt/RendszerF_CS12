using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;

namespace JegyMester.Pages.Cashier.Sell_ticket
{
    public class EditModel : PageModel
    {
        private readonly JegyMester.DataContext.Context.JegyMesterDbContext _context;

        public EditModel(JegyMester.DataContext.Context.JegyMesterDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TicketPurchase TicketPurchase { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var purchase = await _context.TicketPurchases.FindAsync(id);
            if (purchase == null) return NotFound();

            TicketPurchase = purchase;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Attach(TicketPurchase).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketPurchaseExists(TicketPurchase.Id)) return NotFound();
                throw;
            }

            return RedirectToPage("./Details", new { id = TicketPurchase.Id });
        }

        private bool TicketPurchaseExists(int id)
        {
            return _context.TicketPurchases.Any(e => e.Id == id);
        }
    }
}
