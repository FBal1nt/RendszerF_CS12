using JegyMester.DataContext.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScreeningEntity = JegyMester.DataContext.Entities.Screening;

namespace JegyMester.Pages.Screening
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
        public ScreeningEntity Screening { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var screening = await _context.Screenings.FirstOrDefaultAsync(m => m.Id == id);

            if (screening is not null)
            {
                Screening = screening;

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

            var screening = await _context.Screenings.FindAsync(id);
            if (screening != null)
            {
                Screening = screening;
                _context.Screenings.Remove(Screening);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
