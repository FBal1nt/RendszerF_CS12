using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace JegyMester.Pages.User
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly JegyMesterDbContext _db;

        public EditModel(JegyMesterDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public DataContext.Entities.User User { get; set; }

        // ÚJ JELSZÓ MEZŐ
        [BindProperty]
        public string? NewPassword { get; set; }

        // Szerepkörök
        public List<DataContext.Entities.Role> AllRoles { get; set; } = new();
        [BindProperty]
        public List<int> SelectedRoleIds { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            User = await _db.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (User == null)
                return NotFound();

            AllRoles = await _db.Roles.ToListAsync();
            SelectedRoleIds = User.Roles.Select(r => r.Id).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userInDb = await _db.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == User.Id);

            if (userInDb == null)
                return NotFound();

            // Alapadatok frissítése
            userInDb.Name = User.Name;
            userInDb.Email = User.Email;
            userInDb.PhoneNumber = User.PhoneNumber;

            // Jelszó frissítése, ha megadtak újat
            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                userInDb.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            }

            // Szerepkörök frissítése
            userInDb.Roles.Clear();
            if (SelectedRoleIds != null)
            {
                var roles = await _db.Roles
                    .Where(r => SelectedRoleIds.Contains(r.Id))
                    .ToListAsync();

                foreach (var role in roles)
                    userInDb.Roles.Add(role);
            }

            await _db.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
