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
    public class IndexModelCinema : PageModel
    {
        private readonly JegyMester.DataContext.Context.JegyMesterDbContext _context;

        public IndexModelCinema(JegyMester.DataContext.Context.JegyMesterDbContext context)
        {
            _context = context;
        }

        public IList<CinemaEntity> Cinema { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Cinema = await _context.Cinemas.ToListAsync();
        }
    }
}
