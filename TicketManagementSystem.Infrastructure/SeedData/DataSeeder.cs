using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TicketManagementSystem.Infrastructure.Identity;
using TicketManagementSystem.Common.Constants;

namespace TicketManagementSystem.Infrastructure.SeedData
{
    public static class DataSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            const string adminRole = RoleConstants.Admin;
            const string adminEmail = "admin@tms.com";
            const string adminPassword = "Admin@123";

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "System Admin"
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
            }
        }

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles =
            {
            RoleConstants.Admin,
            RoleConstants.Support,
            RoleConstants.EndUser
        };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
