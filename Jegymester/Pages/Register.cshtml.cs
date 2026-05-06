using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class RegisterModel : PageModel
{
    private readonly JegyMesterDbContext _db;

    public RegisterModel(JegyMesterDbContext db)
    {
        _db = db;
    }

    [BindProperty] public string Email { get; set; }
    [BindProperty] public string Password { get; set; }
    [BindProperty] public string ConfirmPassword { get; set; }

    public string ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        // Üres mezők tiltása
        if (string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            ErrorMessage = "Minden mező kitöltése kötelező.";
            return Page();
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "A jelszavak nem egyeznek.";
            return Page();
        }

        var existingUser = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == Email);

        if (existingUser != null)
        {
            ErrorMessage = "Ezzel az email címmel már létezik felhasználó.";
            return Page();
        }

        // Jelszó hash-elése
        var hashed = BCrypt.Net.BCrypt.HashPassword(Password);

        var user = new User
        {
            Email = Email,
            PasswordHash = hashed
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Login");
    }

}
