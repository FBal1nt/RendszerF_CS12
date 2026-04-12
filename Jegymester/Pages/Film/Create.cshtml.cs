using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using JegyMester.DataContext.Context;
using JegyMester.DataContext.Enums;
using FilmEntity = JegyMester.DataContext.Entities.Film;

namespace JegyMester.Pages.Film
{
    public class CreateModelFilm : PageModel
    {
        private readonly JegyMester.DataContext.Context.JegyMesterDbContext _context;

        public CreateModelFilm(JegyMester.DataContext.Context.JegyMesterDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            PopulateGenreSelectList();
            return Page();
        }

        [BindProperty]
        public FilmEntity Film { get; set; } = default!;

        public SelectList GenreSelectList { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                PopulateGenreSelectList();
                return Page();
            }

            _context.Films.Add(Film);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private void PopulateGenreSelectList()
        {
            var values = Enum.GetValues(typeof(GenreType)).Cast<GenreType>()
                .Select(g => new { Id = g, Name = g.ToString() })
                .ToList();

            GenreSelectList = new SelectList(values, "Id", "Name", Film?.Genre);
        }
    }
}
