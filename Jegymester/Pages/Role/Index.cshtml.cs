using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;

namespace JegyMester.Pages.Role
{
    public class IndexModel : PageModel
    {
        private readonly JegyMester.DataContext.Context.JegyMesterDbContext _context;

        public IndexModel(JegyMester.DataContext.Context.JegyMesterDbContext context)
        {
            _context = context;
        }

        public IList<JegyMester.DataContext.Entities.Role> Roles { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Roles = await _context.Roles.ToListAsync();
        }
    }
}
