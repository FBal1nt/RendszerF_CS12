using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;
using JegyMester.DataContext.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ScreeningEntity = JegyMester.DataContext.Entities.Screening;
using CinemaEntity = JegyMester.DataContext.Entities.Cinema;

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

        public IList<CinemaEntity> Cinemas { get; set; } = new List<CinemaEntity>();

        [BindProperty(SupportsGet = true)]
        public int? SelectedCinemaId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? SelectedDate { get; set; }

        public async Task OnGetAsync()
        {
            Cinemas = await _context.Cinemas.OrderBy(c => c.Name).ToListAsync();

            var query = _context.Screenings
                .Include(s => s.Film)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Cinema)
                .Where(s => s.StartTime > DateTime.Now);

            if (SelectedCinemaId.HasValue)
            {
                query = query.Where(s => s.Room.CinemaId == SelectedCinemaId.Value);
            }

            if (SelectedDate.HasValue)
            {
                query = query.Where(s => s.StartTime.Date == SelectedDate.Value.Date);
            }

            Vetitesek = await query
                .OrderBy(s => s.Film.Title)
                .ThenBy(s => s.StartTime)
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

            var purchase = new TicketPurchase
            {
                UserId = userId,
                PurchaseDateTime = DateTime.Now
            };

            _context.TicketPurchases.Add(purchase);
            await _context.SaveChangesAsync();

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