using Microsoft.AspNetCore.Identity;
using PharmacySystem.Core.Constants;

namespace PharmacySystem.Models.Data
{
    public class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = {
                AppConstants.Roles.Admin,
                AppConstants.Roles.Pharmacist,
                AppConstants.Roles.Cashier
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@pharmacy.com";
            var defaultAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (defaultAdmin == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(newAdmin, "Admin@123456");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, AppConstants.Roles.Admin);
                }
            }
        }

    }
}
