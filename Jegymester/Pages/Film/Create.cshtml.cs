using JegyMester.DataContext.Context;
using JegyMester.DataContext.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FilmEntity = JegyMester.DataContext.Entities.Film;

namespace JegyMester.Pages.Film
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly JegyMesterDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(JegyMesterDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult OnGet()
        {
            PopulateGenreSelectList();
            return Page();
        }

        [BindProperty]
        public FilmEntity Film { get; set; } = default!;

        [BindProperty]
        public IFormFile? ImageUpload { get; set; }

        public SelectList GenreSelectList { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                PopulateGenreSelectList();
                return Page();
            }

            if (ImageUpload != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "films");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageUpload.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageUpload.CopyToAsync(fileStream);
                }

                Film.ImagePath = "/images/films/" + uniqueFileName;
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