using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using JegyMester.DataContext.Context;
using ScreeningEntity = JegyMester.DataContext.Entities.Screening;

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

        public async Task OnGetAsync()
        {
            Vetitesek = await _context.Screenings
                .Include(s => s.Film)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Cinema)
                .ToListAsync();
        }
    }
}
