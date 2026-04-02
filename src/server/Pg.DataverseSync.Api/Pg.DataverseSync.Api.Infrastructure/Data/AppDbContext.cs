using Microsoft.EntityFrameworkCore;
using Pg.DataverseSync.Api.Domain;

namespace Pg.DataverseSync.Api.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<User>();
        user.ToTable("User", "Identity");
        user.HasKey(u => u.Id);
        user.Property(u => u.Id).ValueGeneratedOnAdd();
        user.Property(u => u.Username).IsRequired().HasMaxLength(256);
        user.Property(u => u.Email).IsRequired().HasMaxLength(256);
        user.Property(u => u.PasswordHash).IsRequired();
        user.Property(u => u.PasswordSalt).IsRequired();
        user.Property(u => u.CreatedOn).IsRequired();
        user.HasIndex(u => u.Username).IsUnique();
        user.HasIndex(u => u.Email).IsUnique();
    }
}
