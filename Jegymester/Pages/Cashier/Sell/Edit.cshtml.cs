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
    public class EditModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public EditModel(JegyMesterDbContext context)
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
                .AsNoTracking()
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (purchase == null)
                return NotFound();

            TicketPurchase = purchase;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var purchaseFromDb = await _context.TicketPurchases
                .FirstOrDefaultAsync(tp => tp.Id == TicketPurchase.Id);

            if (purchaseFromDb == null)
                return NotFound();

            // Csak a megengedett mezők frissítése
            purchaseFromDb.PurchaseDateTime = TicketPurchase.PurchaseDateTime;

            // UserId NEM módosítható itt
            // GuestEmail / GuestPhone NEM módosítható itt
            // Jegyek NEM módosíthatók itt

            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id = TicketPurchase.Id });
        }
    }
}
