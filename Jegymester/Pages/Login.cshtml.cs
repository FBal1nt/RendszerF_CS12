using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class LoginModel : PageModel
{
    private readonly JegyMesterDbContext _db;
    private readonly IConfiguration _config;

    public LoginModel(JegyMesterDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [BindProperty] public string Email { get; set; }
    [BindProperty] public string Password { get; set; }
    public string ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        // Üres mezők tiltása
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Email és jelszó megadása kötelező.";
            return Page();
        }

        var user = await _db.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == Email);

        if (user == null)
        {
            ErrorMessage = "Hibás email vagy jelszó.";
            return Page();
        }

        // NULL PasswordHash elleni védelem
        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            ErrorMessage = "A felhasználó jelszava hibásan van tárolva.";
            return Page();
        }

        // Jelszó ellenőrzés
        if (!BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash))
        {
            ErrorMessage = "Hibás email vagy jelszó.";
            return Page();
        }


        // JWT token generálása
        var token = GenerateJwtToken(user);

        // Token cookie-ba tétele
        HttpContext.Response.Cookies.Append("auth_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(3)
        });

        // Cookie authentikáció claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        // Cookie-ba bejelentkeztetés
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme))
        );

        return RedirectToPage("/Index");
    }

    private string GenerateJwtToken(User user)
    {
        var keyString = _config["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(keyString))
            throw new Exception("JWT kulcs NULL vagy üres! A konfiguráció nem töltődött be.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email)
    };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
