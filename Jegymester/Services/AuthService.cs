using JegyMester.DataContext.Context;
using JegyMester.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace JegyMester.Services
{
    public class AuthService
    {
        private readonly JegyMesterDbContext _db;

        public User CurrentUser { get; private set; }

        public AuthService(JegyMesterDbContext db)
        {
            _db = db;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return false;

            if (user.Password != password)
                return false;

            CurrentUser = user;
            return true;
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public bool IsLoggedIn => CurrentUser != null;
    }
}
