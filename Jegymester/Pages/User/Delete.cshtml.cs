using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using JegyMester.DataContext.Context;
using UserEntity = JegyMester.DataContext.Entities.User;

namespace JegyMester.Pages.User
{
    [Authorize(Roles = "Admin")]   // Admin backend védelem
    public class DeleteModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public DeleteModel(JegyMesterDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserEntity User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
                return NotFound();

            User = user;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                User = user;

                // Kapcsolatok törlése (User–Role many-to-many)
                user.Roles.Clear();

                _context.Users.Remove(User);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
