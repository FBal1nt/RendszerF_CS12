using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using JegyMester.DataContext.Context;
using UserEntity = JegyMester.DataContext.Entities.User;
using RoleEntity = JegyMester.DataContext.Entities.Role;

namespace JegyMester.Pages.User
{
    [Authorize(Roles = "Admin")]   // Admin backend védelem
    public class CreateModel : PageModel
    {
        private readonly JegyMesterDbContext _context;

        public CreateModel(JegyMesterDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserEntity User { get; set; } = default!;

        [BindProperty]
        public List<int> SelectedRoleIds { get; set; } = new();

        public List<RoleEntity> AllRoles { get; set; } = new();

        public IActionResult OnGet()
        {
            AllRoles = _context.Roles.ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AllRoles = _context.Roles.ToList();
                return Page();
            }

            // Jelszó hash-elése
            if (!string.IsNullOrWhiteSpace(User.PasswordHash))
            {
                User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(User.PasswordHash);
            }

            // Szerepek hozzárendelése
            if (SelectedRoleIds?.Any() ?? false)
            {
                var roles = _context.Roles
                    .Where(r => SelectedRoleIds.Contains(r.Id))
                    .ToList();

                User.Roles = roles;
            }

            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
