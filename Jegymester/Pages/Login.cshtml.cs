using JegyMester.DataContext.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class LoginModel : PageModel
{
    private readonly JegyMesterDbContext _db;

    public LoginModel(JegyMesterDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public string Email { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _db.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == Email);

        if (user == null || user.Password != Password)
        {
            ErrorMessage = "Hibás email vagy jelszó.";
            return Page();
        }

        // Sikeres login → session beállítása
        HttpContext.Session.SetString("UserEmail", Email);
        // Jelöljük, ha az user admin szerepű
        var isAdmin = user.Roles.Any(r => r.Name == "Admin");
        HttpContext.Session.SetString("IsAdmin", isAdmin ? "true" : "false");

        return RedirectToPage("/Index");
    }
}