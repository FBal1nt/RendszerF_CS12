using JegyMester.DataContext.Context;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

namespace JegyMester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<JegyMesterDbContext>(
                options => options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=JegyMesterDB;Trusted_Connection=True;TrustServerCertificate=True;",
                b => b.MigrationsAssembly("JegyMester.DataContext")));

            logger.Info("Building JegyMester");
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
            logger.Info("Running JegyMester");
            app.Run();
            logger.Info("Stopping JegyMester");
        }
    }
}
