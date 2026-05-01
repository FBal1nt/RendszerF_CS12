using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;
using JegyMester.DataContext.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using JegyMester.DataContext.Entities;
using ScreeningEntity = JegyMester.DataContext.Entities.Screening;

namespace JegyMester.Pages
{
    public class VetitesekListaModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public VetitesekListaModel(JegyMesterDbContext context)
        {
            _context = context;
        }

        public IList<ScreeningEntity> Vetitesek { get; set; } = new List<ScreeningEntity>();

        public async Task OnGetAsync()
        {
            Vetitesek = await _context.Screenings
                .Include(s => s.Film)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Cinema)
                .Where(s => s.StartTime > DateTime.Now) // Csak a jövőbeli vetítések
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(int ScreeningId)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
                return RedirectToPage("/Login");

            int userId = int.Parse(userIdString);

            var screening = await _context.Screenings
                .Include(s => s.Film)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.Id == ScreeningId);

            if (screening == null)
                return NotFound();

            // 1) Létrehozzuk a vásárlást
            var purchase = new TicketPurchase
            {
                UserId = userId,
                PurchaseDateTime = DateTime.Now
            };

            _context.TicketPurchases.Add(purchase);
            await _context.SaveChangesAsync(); // kell, hogy legyen purchase.Id

            // 2) Létrehozunk egy jegyet

            var ticket = new DataContext.Entities.Ticket
            {
                ScreeningId = ScreeningId,
                TicketPurchaseId = purchase.Id,
                Type = TicketType.Adult, // ideiglenes, amíg nincs választás
                Valid = true,
                Price = 2000, // ideiglenes ár
                Row = 1,
                SeatNumber = 1
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Sikeres jegyvásárlás!";
            return RedirectToPage();
        }


    }
}
