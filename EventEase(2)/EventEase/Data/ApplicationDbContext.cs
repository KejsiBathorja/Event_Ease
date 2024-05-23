using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;

namespace EventEase.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<EventEase.Models.Category>? Category { get; set; }
        public DbSet<EventEase.Models.Event>? Event { get; set; }
        public DbSet<EventEase.Models.Organizer>? Organizer { get; set; }
        public DbSet<EventEase.Models.ReviewModel>? ReviewModels { get; set; }
        public DbSet<EventEase.Models.UserEvent>? UserEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEvent>()
                .HasNoKey();

            base.OnModelCreating(modelBuilder); // Thirr këtë për të ruajtur konfigurimet e tjera të modelit Identity

            // Përcaktoni një çelës primar për modelin IdentityUserLogin<T>
            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });
        }


    }
}
