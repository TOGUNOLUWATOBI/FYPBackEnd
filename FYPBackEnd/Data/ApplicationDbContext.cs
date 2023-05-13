using FYPBackEnd.Data.Entities;
using FYPBackEnd.Data.Models.Okra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FYPBackEnd.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Otp> Otps { get; set; }
        public DbSet<Account> Accounts { get; set; }

        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(c => c.Account)
                .WithOne(a => a.User)
                .HasForeignKey<Account>(a => a.id);
        }
    }
}
