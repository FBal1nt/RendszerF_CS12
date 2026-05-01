using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly JegyMesterDbContext _db;

    public ProfileModel(JegyMesterDbContext db)
    {
        _db = db;
    }

    [BindProperty] public string Name { get; set; }
    [BindProperty] public string Email { get; set; }
    [BindProperty] public string PhoneNumber { get; set; }
    [BindProperty] public string Password { get; set; }

    public string Message { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // 🔥 Email a JWT tokenből
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
            return RedirectToPage("/Login");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return RedirectToPage("/Login");

        Name = user.Name;
        Email = user.Email;
        PhoneNumber = user.PhoneNumber;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // 🔥 Email a JWT tokenből
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
            return RedirectToPage("/Login");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return RedirectToPage("/Login");

        if (string.IsNullOrWhiteSpace(Email))
        {
            Message = "Az email cím nem lehet üres.";
            return Page();
        }

        // 🔥 Jelszó frissítése, ha meg van adva
        if (!string.IsNullOrWhiteSpace(Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password);
        }

        user.Name = Name;
        user.Email = Email;
        user.PhoneNumber = PhoneNumber;

        await _db.SaveChangesAsync();

        Message = "Profil sikeresen frissítve.";
        return Page();
    }
}
