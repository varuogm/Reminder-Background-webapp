
    using Microsoft.EntityFrameworkCore;
    using RemindMe.Models;
    using System.Collections.Generic;
    using System.Reflection.Emit;
namespace RemindMe
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>()
                .HasIndex(s => s.Endpoint)
                .IsUnique();
        }
    }
}
