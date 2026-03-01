using Microsoft.EntityFrameworkCore;
using JegyMester.DataContext.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
