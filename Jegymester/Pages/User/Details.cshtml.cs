using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using JegyMester.DataContext.Context;

namespace JegyMester.Pages.User
{
    public class DetailsModel : PageModel
    {
        private readonly JegyMester.DataContext.Context.JegyMesterDbContext _context;

        public DetailsModel(JegyMester.DataContext.Context.JegyMesterDbContext context)
        {
            _context = context;
        }

        public DataContext.Entities.User User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);

            if (user is not null)
            {
                User = user;

                return Page();
            }

            return NotFound();
        }
    }
}
