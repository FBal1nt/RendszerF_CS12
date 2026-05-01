using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using JegyMester.DataContext.Context;
using UserEntity = JegyMester.DataContext.Entities.User;

namespace JegyMester.Pages.User
{
    [Authorize(Roles = "Admin")]   // Admin backend védelem
    public class IndexModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public IndexModel(JegyMesterDbContext context)
        {
            _context = context;
        }

        public IList<UserEntity> User { get; set; } = default!;

        public async Task OnGetAsync()
        {
            User = await _context.Users
                .Include(u => u.Roles)
                .ToListAsync();
        }
    }
}
