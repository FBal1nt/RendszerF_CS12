using JegyMester.DataContext.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilmEntity = JegyMester.DataContext.Entities.Film;

namespace JegyMester.Pages.Film
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly JegyMester.DataContext.Context.JegyMesterDbContext _context;

        public DeleteModel(JegyMester.DataContext.Context.JegyMesterDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FilmEntity Film { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films.FirstOrDefaultAsync(m => m.Id == id);

            if (film is not null)
            {
                Film = film;

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

            var film = await _context.Films.FindAsync(id);
            if (film != null)
            {
                Film = film;
                _context.Films.Remove(Film);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
