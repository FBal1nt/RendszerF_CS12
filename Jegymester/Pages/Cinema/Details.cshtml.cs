using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using JegyMester.DataContext.Context;
using CinemaEntity = JegyMester.DataContext.Entities.Cinema;

namespace JegyMester.Pages.Cinema
{
    public class DetailsModelCinema : PageModel
    {
        private readonly JegyMester.DataContext.Context.JegyMesterDbContext _context;

        public DetailsModelCinema(JegyMester.DataContext.Context.JegyMesterDbContext context)
        {
            _context = context;
        }

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
    }
}
