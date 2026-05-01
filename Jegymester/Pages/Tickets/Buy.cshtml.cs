using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;
using JegyMester.DataContext.Enums;
using System.Security.Claims;

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

            TakenSeats = await _context.Tickets
                .Where(t => t.ScreeningId == id && t.Valid == true)
                .Select(t => new ValueTuple<int, int>(t.Row, t.SeatNumber))
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // JWT user ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = userIdClaim != null ? int.Parse(userIdClaim) : null;

            // Vendég vásárlás ellenőrzése
            if (userId == null)
            {
                if (string.IsNullOrWhiteSpace(GuestEmail) || string.IsNullOrWhiteSpace(GuestPhone))
                {
                    TempData["Error"] = "Vendég vásárlás esetén kötelező az e-mail és a telefonszám megadása.";
                    return RedirectToPage(new { id = ScreeningId });
                }
            }

            // Vetítés betöltése
            var screening = await _context.Screenings
                .Include(s => s.Film)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.Id == ScreeningId);

            if (screening == null)
                return NotFound();

            if (screening.StartTime <= DateTime.Now)
            {
                TempData["Error"] = "Ez a vetítés már elkezdődött vagy lejárt, jegyvásárlás nem lehetséges.";
                return RedirectToPage(new { id = ScreeningId });
            }

            // Helyfoglalás ellenőrzése
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

            // Vásárlás létrehozása
            var purchase = new TicketPurchase
            {
                UserId = userId,
                GuestEmail = userId == null ? GuestEmail : null,
                GuestPhone = userId == null ? GuestPhone : null,
                PurchaseDateTime = DateTime.Now
            };

            _context.TicketPurchases.Add(purchase);
            await _context.SaveChangesAsync();

            // Jegy létrehozása
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

            if (userId != null)
                return RedirectToPage("/Tickets/MyTickets");

            return RedirectToPage("/Index");
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
