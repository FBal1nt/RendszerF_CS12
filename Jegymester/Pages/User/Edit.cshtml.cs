using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JegyMester.DataContext.Context;
using UserEntity = JegyMester.DataContext.Entities.User;
using RoleEntity = JegyMester.DataContext.Entities.Role;

namespace JegyMester.Pages.User
{
    public class EditModel : PageModel
    {
        private readonly JegyMester.DataContext.Context.JegyMesterDbContext _context;

        public EditModel(JegyMester.DataContext.Context.JegyMesterDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserEntity User { get; set; } = default!;

        [BindProperty]
        public List<int> SelectedRoleIds { get; set; } = new();

        public List<RoleEntity> AllRoles { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user =  await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            User = user;
            AllRoles = await _context.Roles.ToListAsync();
            SelectedRoleIds = user.Roles.Select(r => r.Id).ToList();
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var userFromDb = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == User.Id);
                if (userFromDb == null)
                {
                    return NotFound();
                }

                // update scalar properties
                userFromDb.Name = User.Name;
                userFromDb.Email = User.Email;
                userFromDb.PhoneNumber = User.PhoneNumber;
                // update password only when a new value was provided
                if (!string.IsNullOrEmpty(User.Password))
                {
                    userFromDb.Password = User.Password;
                }

                // update roles
                var roles = SelectedRoleIds?.Any() == true
                    ? _context.Roles.Where(r => SelectedRoleIds.Contains(r.Id)).ToList()
                    : new List<RoleEntity>();

                userFromDb.Roles.Clear();
                foreach (var r in roles)
                {
                    userFromDb.Roles.Add(r);
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.Id))
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

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
