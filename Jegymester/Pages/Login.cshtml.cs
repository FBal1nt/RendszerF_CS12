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
            .FirstOrDefaultAsync(u => u.Email == Email);

        if (user == null || user.Password != Password)
        {
            ErrorMessage = "Hibás email vagy jelszó.";
            return Page();
        }

        // Sikeres login → átirányítás
        HttpContext.Session.SetString("UserEmail", Email);
        return RedirectToPage("/Index");
    }
}