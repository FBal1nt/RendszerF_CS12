using JegyMester.DataContext.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using NLog;
using NLog.Web;
using Microsoft.AspNetCore.Identity;
using JegyMester.DataContext.Entities;

namespace JegyMester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("JegyMesterContextConnection") ?? throw new InvalidOperationException("Connection string 'JegyMesterContextConnection' not found.");;

            var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            // Add services to the container.
            builder.Services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/Pages/Role", "RequireAdminRole");
            });

            builder.Services.AddDbContext<JegyMesterDbContext>(
                options => options.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;Command Timeout=0",
                b => b.MigrationsAssembly("JegyMester.DataContext")));

            builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<Role>()
                .AddEntityFrameworkStores<JegyMesterDbContext>();

            // Politika létrehozása (opcionális, de ajánlott)
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "JegyMester API",
                    Version = "v1",
                    Description = "JegyMester API"
                });
            });

            

            logger.Info("Building JegyMester");
            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
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
