using Microsoft.EntityFrameworkCore;
using Pg.DataverseSync.Api.Application.Model;
using Pg.DataverseSync.Api.Domain;

namespace Pg.DataverseSync.Api.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

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

        var refreshToken = modelBuilder.Entity<RefreshToken>();
        refreshToken.ToTable("RefreshToken", "Identity");
        refreshToken.HasKey(rt => rt.Id);
        refreshToken.Property(rt => rt.Id).ValueGeneratedOnAdd();
        refreshToken.Property(rt => rt.UserId).IsRequired();
        refreshToken.Property(rt => rt.Token).IsRequired().HasMaxLength(500);
        refreshToken.Property(rt => rt.ExpiresAt).IsRequired();
        refreshToken.Property(rt => rt.CreatedAt).IsRequired();
        refreshToken.HasIndex(rt => rt.Token).IsUnique();
        refreshToken.HasIndex(rt => rt.UserId);
    }
}
