using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CinemaEntity = JegyMester.DataContext.Entities.Cinema;
using FilmEntity = JegyMester.DataContext.Entities.Film;
using ScreeningEntity = JegyMester.DataContext.Entities.Screening;

namespace JegyMester.Pages
{
    [Authorize(Roles = "Admin")]
    public class ScreeningListByFilmModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public ScreeningListByFilmModel(JegyMesterDbContext context)
        {
            _context = context;
        }

        public FilmEntity Film { get; set; }
        public IList<ScreeningEntity> Screenings { get; set; }
        public IList<CinemaEntity> Cinemas { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedCinemaId { get; set; }

        // Új dátum szûrõ property
        [BindProperty(SupportsGet = true)]
        public DateTime? SelectedDate { get; set; }

        public async Task<IActionResult> OnGetAsync(int movieId)
        {
            Film = await _context.Films.FirstOrDefaultAsync(m => m.Id == movieId);

            if (Film == null)
            {
                return NotFound();
            }

            Cinemas = await _context.Cinemas.OrderBy(c => c.Name).ToListAsync();

            var query = _context.Screenings
                .Include(s => s.Room)
                .ThenInclude(r => r.Cinema)
                .Where(s => s.FilmId == movieId && s.StartTime > DateTime.Now);

            // Szûrés mozi alapján
            if (SelectedCinemaId.HasValue)
            {
                query = query.Where(s => s.Room.CinemaId == SelectedCinemaId.Value);
            }

            // ÚJ: Szûrés dátum alapján
            if (SelectedDate.HasValue)
            {
                query = query.Where(s => s.StartTime.Date == SelectedDate.Value.Date);
            }

            Screenings = await query
                .OrderBy(s => s.Room.Cinema.Name)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            return Page();
        }
    }
}