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
    public class DetailsModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public DetailsModel(JegyMesterDbContext context)
        {
            _context = context;
        }

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
    }
}
