using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;
using JegyMester.DataContext.Enums;

namespace JegyMester.Pages.Tickets
{
    public class SelectedTicketDto
    {
        public int Row { get; set; }
        public int SeatNumber { get; set; }
        public TicketType Type { get; set; }
        public decimal Price { get; set; }
    }
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

        [BindProperty]
        public string SelectedTicketsJson { get; set; } // Ide érkezik a JSON a frontenden keresztül

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



        //public async Task<IActionResult> OnPostAsync()
        //{
        //    var userIdString = HttpContext.Session.GetString("UserId");

        //    int? userId = userIdString != null ? int.Parse(userIdString) : null;

        //    if (userId == null)
        //    {
        //        if (string.IsNullOrWhiteSpace(GuestEmail) || string.IsNullOrWhiteSpace(GuestPhone))
        //        {
        //            TempData["Error"] = "Vendég vásárlás esetén kötelező az e-mail és a telefonszám megadása.";
        //            return RedirectToPage(new { id = ScreeningId });
        //        }
        //    }




        //    // 1) Vetítés betöltése
        //    var screening = await _context.Screenings
        //        .Include(s => s.Film)
        //        .Include(s => s.Room)
        //        .FirstOrDefaultAsync(s => s.Id == ScreeningId);
        //    if (screening.StartTime <= DateTime.Now)
        //    {
        //        TempData["Error"] = "Ez a vetítés már elkezdődött vagy lejárt, jegyvásárlás nem lehetséges.";
        //        return RedirectToPage(new { id = ScreeningId });
        //    }



        //    if (screening == null)
        //        return NotFound();

        //    // 2) Vásárlás létrehozása
        //    var purchase = new TicketPurchase
        //    {
        //        UserId = userId,
        //        GuestEmail = userId == null ? GuestEmail : null,
        //        GuestPhone = userId == null ? GuestPhone : null,
        //        PurchaseDateTime = DateTime.Now
        //    };

        //    _context.TicketPurchases.Add(purchase);
        //    await _context.SaveChangesAsync(); // kell, hogy legyen purchase.Id

        //    // 2.5) Helyfoglalás ellenőrzése  ⬇⬇⬇ IDE
        //    bool seatTaken = await _context.Tickets.AnyAsync(t =>
        //        t.ScreeningId == ScreeningId &&
        //        t.Row == Row &&
        //        t.SeatNumber == SeatNumber &&
        //        t.Valid == true
        //    );

        //    if (seatTaken)
        //    {
        //        TempData["Error"] = "Ez a hely már foglalt.";
        //        return RedirectToPage(new { id = ScreeningId });
        //    }

        //    // 3) Jegy létrehozása
        //    var ticket = new Ticket
        //    {
        //        ScreeningId = ScreeningId,
        //        TicketPurchaseId = purchase.Id,
        //        Type = SelectedType,
        //        Valid = true,
        //        Price = GetPrice(SelectedType),
        //        Row = Row,
        //        SeatNumber = SeatNumber
        //    };

        //    _context.Tickets.Add(ticket);
        //    await _context.SaveChangesAsync();

        //    TempData["Message"] = "Sikeres jegyvásárlás!";
        //    if (userId != null)
        //    {
        //        // Bejelentkezett felhasználó → MyTickets
        //        return RedirectToPage("/Tickets/MyTickets");
        //    }
        //    else
        //    {
        //        // Vendég → főoldal
        //        return RedirectToPage("/Index");
        //    }
        //}



        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Validáció: Van-e kijelölt jegy?
            if (string.IsNullOrEmpty(SelectedTicketsJson))
            {
                TempData["Error"] = "Válassz legalább egy helyet!";
                return RedirectToPage(new { id = ScreeningId });
            }

            //var selectedTickets = System.Text.Json.JsonSerializer.Deserialize<List<SelectedTicketDto>>(
            //    SelectedTicketsJson,
            //    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var selectedTickets = System.Text.Json.JsonSerializer.Deserialize<List<SelectedTicketDto>>(
            SelectedTicketsJson,
            new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    // Ez a sor mondja meg neki, hogy a szöveget alakítsa Enummá
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                });

            // 2. Felhasználó / Vendég kezelése
            var userIdString = HttpContext.Session.GetString("UserId");
            int? userId = userIdString != null ? int.Parse(userIdString) : null;

            if (userId == null)
            {
                if (string.IsNullOrWhiteSpace(GuestEmail) || string.IsNullOrWhiteSpace(GuestPhone))
                {
                    TempData["Error"] = "Vendégként kötelező az e-mail és a telefonszám!";
                    return RedirectToPage(new { id = ScreeningId });
                }
            }

            // 3. Vásárlás rögzítése
            var purchase = new TicketPurchase
            {
                UserId = userId,
                GuestEmail = userId == null ? GuestEmail : null,
                GuestPhone = userId == null ? GuestPhone : null,
                PurchaseDateTime = DateTime.Now
            };

            // 4. Jegyek hozzáadása és egyenkénti ellenőrzése
            foreach (var item in selectedTickets)
            {
                bool seatTaken = await _context.Tickets.AnyAsync(t =>
                    t.ScreeningId == ScreeningId &&
                    t.Row == item.Row &&
                    t.SeatNumber == item.SeatNumber &&
                    t.Valid == true);

                if (seatTaken)
                {
                    TempData["Error"] = $"A(z) {item.Row}. sor {item.SeatNumber}. szék már foglalt!";
                    return RedirectToPage(new { id = ScreeningId });
                }

                purchase.Tickets.Add(new Ticket
                {
                    ScreeningId = ScreeningId,
                    Type = item.Type,
                    Valid = true,
                    Price = item.Price,
                    Row = item.Row,
                    SeatNumber = item.SeatNumber
                });
            }

            _context.TicketPurchases.Add(purchase);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Sikeres vásárlás!";
            return userId != null ? RedirectToPage("/Tickets/MyTickets") : RedirectToPage("/Index");
        }


        public decimal GetPrice(TicketType type)
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
