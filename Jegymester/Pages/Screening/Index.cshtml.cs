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
    public class IndexModelScreening : PageModel
    {
        private readonly JegyMester.DataContext.Context.JegyMesterDbContext _context;

        public IndexModelScreening(JegyMester.DataContext.Context.JegyMesterDbContext context)
        {
            _context = context;
        }

        public IList<ScreeningEntity> Screening { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Screening = await _context.Screenings
                .Include(s => s.Film)
                .Include(s => s.Room).ToListAsync();
        }
    }
}
