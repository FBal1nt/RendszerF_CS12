using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;
using JegyMester.DataContext.Enums;

namespace JegyMester.Pages.Tickets
{
    public class BuyModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public BuyModel(JegyMesterDbContext context)
        {
            _context = context;
        }

        public DataContext.Entities.Screening Screening { get; set; }

        [BindProperty]
        public int ScreeningId { get; set; }
        public bool IsExpired => Screening != null && Screening.StartTime <= DateTime.Now;


        [BindProperty]
        public TicketType SelectedType { get; set; }

        [BindProperty]
        public int Row { get; set; }

        [BindProperty]
        public int SeatNumber { get; set; }
        public decimal CurrentPrice => GetPrice(SelectedType);

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ScreeningId = id;

            Screening = await _context.Screenings
                .Include(s => s.Film)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (Screening == null)
                return NotFound();

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
                return RedirectToPage("/Login");

            int userId = int.Parse(userIdString);

            // 1) Vetítés betöltése
            var screening = await _context.Screenings
                .Include(s => s.Film)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.Id == ScreeningId);
            if (screening.StartTime <= DateTime.Now)
            {
                TempData["Error"] = "Ez a vetítés már elkezdődött vagy lejárt, jegyvásárlás nem lehetséges.";
            }


            if (screening == null)
                return NotFound();

            // 2) Vásárlás létrehozása
            var purchase = new TicketPurchase
            {
                UserId = userId,
                PurchaseDateTime = DateTime.Now
            };

            _context.TicketPurchases.Add(purchase);
            await _context.SaveChangesAsync(); // kell, hogy legyen purchase.Id

            // 3) Jegy létrehozása
            var ticket = new Ticket
            {
                ScreeningId = ScreeningId,
                TicketPurchaseId = purchase.Id,
                Type = SelectedType,
                Valid = true,
                Price = GetPrice(SelectedType),
                Row = Row,
                SeatNumber = SeatNumber
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Sikeres jegyvásárlás!";
            return RedirectToPage("/Tickets/MyTickets");
        }

        private decimal GetPrice(TicketType type)
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
