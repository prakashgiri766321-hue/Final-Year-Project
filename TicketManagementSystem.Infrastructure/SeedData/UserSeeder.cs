using Microsoft.AspNetCore.Identity;
using TicketManagementSystem.Common.Constants;
using TicketManagementSystem.Infrastructure.Identity;

namespace TicketManagementSystem.Infrastructure.SeedData;

public static class UserSeeder
{
    public static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        await CreateUser(userManager, "admin@tms.com", "Admin@123", RoleConstants.Admin);
        await CreateUser(userManager, "support@tms.com", "Support@123", RoleConstants.Support);
        await CreateUser(userManager, "user@tms.com", "User@123", RoleConstants.EndUser);
    }

    private static async Task CreateUser(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string role)
    {
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = email 
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}