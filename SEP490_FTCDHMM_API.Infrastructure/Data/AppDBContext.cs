using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Configurations;
using SEP490_FTCDHMM_API.Infrastructure.Persistence.SeedData;

namespace SEP490_FTCDHMM_API.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<EmailOtp> EmailOtps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure custom AppUser-AppRole relationship while keeping Identity tables
            modelBuilder.Entity<AppUser>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

            // Keep Identity tables but configure them properly
            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.ApplyConfiguration(new AppUserConfiguration());
            modelBuilder.ApplyConfiguration(new AppRoleConfiguration());
            modelBuilder.ApplyConfiguration(new EmailOtpConfiguration());

            modelBuilder.SeedRoles();
        }
    }
}
