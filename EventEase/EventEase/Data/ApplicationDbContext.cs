using EventEase.Data.EF;
using EventEase.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        internal readonly object Login;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Login> login { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<Login> login = new List<Login>();
            login.Add(new Login { id = 1, Username = "Admin", Password = "Admin", Role = Role.Admin });
            modelBuilder.Entity<Login>().HasData(login);

        }
    }
}
