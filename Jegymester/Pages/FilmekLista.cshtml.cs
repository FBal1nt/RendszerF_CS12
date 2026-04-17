using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using JegyMester.DataContext.Context;
using FilmEntity = JegyMester.DataContext.Entities.Film;

namespace JegyMester.Pages
{
    public class FilmekListaModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public FilmekListaModel(JegyMesterDbContext context)
        {
            _context = context;
        }

        public IList<FilmEntity> Filmek { get; set; } = new List<FilmEntity>();

        public async Task OnGetAsync()
        {
            Filmek = await _context.Films.ToListAsync();
        }
    }
}
