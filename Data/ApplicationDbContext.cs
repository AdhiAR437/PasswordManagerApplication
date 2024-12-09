using Microsoft.EntityFrameworkCore;
using PasswordManagerApplication.Models;

namespace PasswordManagerApplication.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users_tb { get; set; }
        public DbSet<PasswordEntry> PasswordEntries_tb { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<PasswordEntry>()
                .HasOne(pe => pe.User)
                .WithMany(u => u.PasswordEntries)
                .HasForeignKey(pe => pe.UserId);

            modelBuilder.Entity<PasswordEntry>()
                .HasIndex(pe => new { pe.UserId, pe.Title })  // Composite index on UserId and Title
                .IsUnique();

            modelBuilder.Entity<PasswordEntry>()
                .HasIndex(pe => new { pe.UserId, pe.Website, pe.Username })  // Composite index on UserId and Title
                .IsUnique();
        }
    }
}
