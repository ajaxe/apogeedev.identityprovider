using ApogeeDev.IdentityProvider.Host.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace ApogeeDev.IdentityProvider.Host.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AppDbSettings> AppDbSettings { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<AppUserClaim> AppUserClaims { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppDbSettings>();
        modelBuilder.Entity<AppUser>();
        modelBuilder.Entity<AppUserClaim>();
    }
}