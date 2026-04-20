using JegyMester.DataContext.Context;
using JegyMester.Services;
using Microsoft.EntityFrameworkCore;

namespace JegyMester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();

            // Ez majd ha meglesz az adatbázis akkor kell
            builder.Services.AddDbContext<JegyMesterDbContext>(options => options.UseSqlServer("Server=(local);Database=JegyMesterDB;Trusted_Connection=True;TrustServerCertificate=True;"));
            
            builder.Services.AddScoped<AuthService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();
            app.UseSession();

            app.Run();
        }
    }
}
