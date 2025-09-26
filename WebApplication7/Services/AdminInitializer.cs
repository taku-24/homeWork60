using Microsoft.AspNetCore.Identity;
using WebApplication7.Models;

namespace WebApplication7.Services;

public class AdminInitializer
{
    public static async Task SeedAdminUser(RoleManager<IdentityRole<int>> roleManager, UserManager<User> userManager)
    {
        string adminEmail = "admin@admin.com";
        string adminPassword = "Admin123%";
        var roles = new [] {"admin", "user"};

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            User admin = new User { Email = adminEmail, UserName = adminEmail, DateOfBirth = DateTime.UtcNow };
            IdentityResult result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "admin");
            }
        }

    }
    
}