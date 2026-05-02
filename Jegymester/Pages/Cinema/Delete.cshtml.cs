using JegyMester.DataContext.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaEntity = JegyMester.DataContext.Entities.Cinema;

namespace JegyMester.Pages.Cinema
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
        public CinemaEntity Cinema { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cinema = await _context.Cinemas.FirstOrDefaultAsync(m => m.Id == id);

            if (cinema is not null)
            {
                Cinema = cinema;

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

            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema != null)
            {
                Cinema = cinema;
                _context.Cinemas.Remove(Cinema);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
