using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Server.Models;
using SkillSnap.Shared.Models;

namespace SkillSnap.Server.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var db = provider.GetRequiredService<SkillSnapDbContext>();
        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = provider.GetRequiredService<ILogger<SkillSnapDbContext>>();

        try
        {
            // Create roles
            var roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"Role '{role}' created.");
                    }
                }
            }

            // Create sample users
            var adminEmail = "admin@skillsnap.local";
            var userEmail = "user@skillsnap.local";

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
                var result = await userManager.CreateAsync(admin, "Password123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    logger.LogInformation($"Admin user '{adminEmail}' created.");
                }
            }

            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                user = new ApplicationUser { UserName = userEmail, Email = userEmail };
                var result = await userManager.CreateAsync(user, "Password123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                    logger.LogInformation($"User '{userEmail}' created.");
                }
            }

            // Seed portfolio items if empty
            if (!await db.PortfolioItems.AnyAsync())
            {
                db.PortfolioItems.AddRange(
                    new PortfolioItem 
                    { 
                        Title = "Personal Website", 
                        Description = "A responsive personal website built with Blazor WebAssembly.", 
                        UserId = user?.Id, 
                        CreatedAt = DateTime.UtcNow.AddDays(-10) 
                    },
                    new PortfolioItem 
                    { 
                        Title = "E-Commerce API", 
                        Description = "RESTful API built with ASP.NET Core 8, Entity Framework Core, and SQL Server.", 
                        UserId = admin?.Id, 
                        CreatedAt = DateTime.UtcNow.AddDays(-5) 
                    },
                    new PortfolioItem 
                    { 
                        Title = "Mobile App (MAUI)", 
                        Description = "Cross-platform mobile application using .NET MAUI with user authentication.", 
                        UserId = user?.Id, 
                        CreatedAt = DateTime.UtcNow.AddDays(-2) 
                    }
                );
                await db.SaveChangesAsync();
                logger.LogInformation("Portfolio items seeded.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error initializing database. Continuing startup.");
        }
    }
}
