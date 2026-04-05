using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using JegyMester.DataContext.Context;
using ScreeningEntity = JegyMester.DataContext.Entities.Screening;

namespace JegyMester.Pages.Screening
{
    public class DetailsModel : PageModel
    {
        private readonly JegyMester.DataContext.Context.JegyMesterDbContext _context;

        public DetailsModel(JegyMester.DataContext.Context.JegyMesterDbContext context)
        {
            _context = context;
        }

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
    }
}
