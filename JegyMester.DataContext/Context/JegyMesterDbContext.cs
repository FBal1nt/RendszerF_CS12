using Microsoft.EntityFrameworkCore;
using JegyMester.DataContext.Entities;

namespace JegyMester.DataContext.Context
{
    public class JegyMesterDbContext : DbContext
    {
        public DbSet<Error> Errors { get; set; }
        public DbSet<Film> Films { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Screening> Screenings { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketPurchase> TicketPurchases { get; set; }
        public DbSet<User> Users { get; set; }

        public JegyMesterDbContext(DbContextOptions<JegyMesterDbContext> options)
            : base(options)
        {
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<Enum>()
                .HaveConversion<string>();

            configurationBuilder
                .Properties<decimal>()
                .HavePrecision(18, 2);
        }
    }
}