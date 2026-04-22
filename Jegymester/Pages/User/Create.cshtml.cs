using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using JegyMester.DataContext.Context;
using UserEntity = JegyMester.DataContext.Entities.User;
using RoleEntity = JegyMester.DataContext.Entities.Role;
using Microsoft.EntityFrameworkCore;

namespace JegyMester.Pages.User
{
    public class CreateModel : PageModel
    {
        private readonly JegyMester.DataContext.Context.JegyMesterDbContext _context;

        public CreateModel(JegyMester.DataContext.Context.JegyMesterDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            AllRoles = _context.Roles.ToList();
            return Page();
        }

        [BindProperty]
        public UserEntity User { get; set; } = default!;
        [BindProperty]
        public List<int> SelectedRoleIds { get; set; } = new();

        public List<RoleEntity> AllRoles { get; set; } = new();
        public string ErrorMessage { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AllRoles = await _context.Roles.ToListAsync();
                return Page();
            }

            if (SelectedRoleIds?.Any() ?? false)
            {
                var roles = await _context.Roles.Where(r => SelectedRoleIds.Contains(r.Id)).ToListAsync();
                User.Roles = roles;
            }

            var existingUser = await _context.Users.AnyAsync(u => u.Email == User.Email);
            if (existingUser)
            {
                ModelState.AddModelError("User.Email", "Ezzel az email címmel már létezik felhasználó.");

                AllRoles = await _context.Roles.ToListAsync();
                return Page();
            }

            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
