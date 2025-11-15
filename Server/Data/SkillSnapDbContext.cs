using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Server.Models;
using SkillSnap.Shared.Models;

namespace SkillSnap.Server.Data;

public class SkillSnapDbContext : IdentityDbContext<ApplicationUser>
{
    public SkillSnapDbContext(DbContextOptions<SkillSnapDbContext> options) : base(options)
    {
    }

    public DbSet<PortfolioItem> PortfolioItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure PortfolioItem
        builder.Entity<PortfolioItem>().HasKey(p => p.Id);
        builder.Entity<PortfolioItem>()
            .Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);
        builder.Entity<PortfolioItem>()
            .Property(p => p.Description)
            .HasMaxLength(2000);
    }
}
