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
    public class DeleteModel : PageModel
    {
        private readonly JegyMester.DataContext.Context.JegyMesterDbContext _context;

        public DeleteModel(JegyMester.DataContext.Context.JegyMesterDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TicketPurchase TicketPurchase { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketpurchase = await _context.TicketPurchases.FirstOrDefaultAsync(m => m.Id == id);

            if (ticketpurchase is not null)
            {
                TicketPurchase = ticketpurchase;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketpurchase = await _context.TicketPurchases.FindAsync(id);
            if (ticketpurchase != null)
            {
                TicketPurchase = ticketpurchase;
                _context.TicketPurchases.Remove(TicketPurchase);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
