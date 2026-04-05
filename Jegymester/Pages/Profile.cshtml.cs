using JegyMester.DataContext.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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
        var email = HttpContext.Session.GetString("UserEmail");
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
        var sessionEmail = HttpContext.Session.GetString("UserEmail");
        if (sessionEmail == null)
            return RedirectToPage("/Login");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == sessionEmail);
        
        if (user == null)
            return RedirectToPage("/Login");
        if (string.IsNullOrWhiteSpace(Email))
        {
            Message = "Az email cím nem lehet üres.";
            return Page();
        }
        if (string.IsNullOrWhiteSpace(Password))
        {
            Message = "A jelszó nem lehet üres.";
            return Page();
        }
        // Adatok frissítése
        user.Name = Name;
        user.Email = Email;
        user.PhoneNumber = PhoneNumber;

        // Jelszó frissítése, ha meg van adva
        if (!string.IsNullOrWhiteSpace(Password))
        {
            user.Password = Password; // később hash-elünk
        }


        await _db.SaveChangesAsync();

        // Session frissítése, ha email változott
        HttpContext.Session.SetString("UserEmail", Email);

        Message = "Profil sikeresen frissítve.";

        return Page();
    }
}
