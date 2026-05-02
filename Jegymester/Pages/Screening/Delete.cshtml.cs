using JegyMester.DataContext.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ScreeningEntity = JegyMester.DataContext.Entities.Screening;

namespace JegyMester.Pages.Screening
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public DeleteModel(JegyMesterDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ScreeningEntity Screening { get; set; } = default!;

        // Új tulajdonság az indoklásnak, kötelező kitöltéssel
        [BindProperty]
        [Required(ErrorMessage = "A törlés indoklása kötelező! Ez fog megjelenni a felhasználók jegyein.")]
        public string CancellationReason { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var screening = await _context.Screenings
                .Include(s => s.Film)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (screening is not null)
            {
                Screening = screening;
                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            if (!ModelState.IsValid)
            {
                Screening = await _context.Screenings
                    .Include(s => s.Film)
                    .Include(s => s.Room)
                    .FirstOrDefaultAsync(m => m.Id == id);
                return Page();
            }

            var screening = await _context.Screenings.FindAsync(id);
            if (screening != null)
            {
                var tickets = await _context.Tickets.Where(t => t.ScreeningId == id).ToListAsync();

                foreach (var ticket in tickets)
                {
                    ticket.CancellationReason = CancellationReason;
                    ticket.ScreeningId = null;
                }

                _context.Screenings.Remove(screening);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}