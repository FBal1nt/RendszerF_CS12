using JegyMester.DataContext.Context;
using JegyMester.DataContext.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FilmEntity = JegyMester.DataContext.Entities.Film;

namespace JegyMester.Pages.Film
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly JegyMesterDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public EditModel(JegyMesterDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public FilmEntity Film { get; set; } = default!;

        [BindProperty]
        public IFormFile? ImageUpload { get; set; }

        public SelectList GenreSelectList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films.FirstOrDefaultAsync(m => m.Id == id);
            if (film == null)
            {
                return NotFound();
            }
            Film = film;
            PopulateGenreSelectList();
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                PopulateGenreSelectList();
                return Page();
            }

            if (ImageUpload != null)
            {
                if (!string.IsNullOrEmpty(Film.ImagePath))
                {
                    string oldFilePath = Path.Combine(_environment.WebRootPath, Film.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

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

            _context.Attach(Film).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FilmExists(Film.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private void PopulateGenreSelectList()
        {
            var values = Enum.GetValues(typeof(GenreType)).Cast<GenreType>()
                .Select(g => new { Id = g, Name = g.ToString() })
                .ToList();

            GenreSelectList = new SelectList(values, "Id", "Name", Film?.Genre);
        }

        private bool FilmExists(int id)
        {
            return _context.Films.Any(e => e.Id == id);
        }
    }
}