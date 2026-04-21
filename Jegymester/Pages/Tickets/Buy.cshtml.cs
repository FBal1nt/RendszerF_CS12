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

        [BindProperty]
        public string? GuestEmail { get; set; }

        [BindProperty]
        public string? GuestPhone { get; set; }

        public List<(int Row, int Seat)> TakenSeats { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(int id)
        {
            ScreeningId = id;

            Screening = await _context.Screenings
                .Include(s => s.Film)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (Screening == null)
                return NotFound();

            // Foglalt helyek lekérése – EZ KELL IDE
            TakenSeats = await _context.Tickets
                .Where(t => t.ScreeningId == id && t.Valid == true)
                .Select(t => new ValueTuple<int, int>(t.Row, t.SeatNumber))
                .ToListAsync();

            return Page();
        }



        public async Task<IActionResult> OnPostAsync()
        {
            var userIdString = HttpContext.Session.GetString("UserId");

            int? userId = userIdString != null ? int.Parse(userIdString) : null;

            if (userId == null)
            {
                if (string.IsNullOrWhiteSpace(GuestEmail) || string.IsNullOrWhiteSpace(GuestPhone))
                {
                    TempData["Error"] = "Vendég vásárlás esetén kötelező az e-mail és a telefonszám megadása.";
                    return RedirectToPage(new { id = ScreeningId });
                }
            }

            


            // 1) Vetítés betöltése
            var screening = await _context.Screenings
                .Include(s => s.Film)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.Id == ScreeningId);
            if (screening.StartTime <= DateTime.Now)
            {
                TempData["Error"] = "Ez a vetítés már elkezdődött vagy lejárt, jegyvásárlás nem lehetséges.";
                return RedirectToPage(new { id = ScreeningId });
            }



            if (screening == null)
                return NotFound();

            // 2) Vásárlás létrehozása
            var purchase = new TicketPurchase
            {
                UserId = userId,
                GuestEmail = userId == null ? GuestEmail : null,
                GuestPhone = userId == null ? GuestPhone : null,
                PurchaseDateTime = DateTime.Now
            };

            _context.TicketPurchases.Add(purchase);
            await _context.SaveChangesAsync(); // kell, hogy legyen purchase.Id

            // 2.5) Helyfoglalás ellenőrzése  ⬇⬇⬇ IDE
            bool seatTaken = await _context.Tickets.AnyAsync(t =>
                t.ScreeningId == ScreeningId &&
                t.Row == Row &&
                t.SeatNumber == SeatNumber &&
                t.Valid == true
            );

            if (seatTaken)
            {
                TempData["Error"] = "Ez a hely már foglalt.";
                return RedirectToPage(new { id = ScreeningId });
            }

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
