using Blockchain.Domain.Entities;
using Blockchain.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            // Seed roles
            foreach (var role in Enum.GetNames(typeof(UserRole)))
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }

            // Seed super admin
            const string adminEmail = "admin@blockchainapi.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new AppUser
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    Email = adminEmail,
                    UserName = "superadmin",
                    Role = UserRole.SuperAdmin,
                    IsActive = true,
                    EmailVerified = true,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(admin, "Admin@123456!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, UserRole.SuperAdmin.ToString());
            }
        }
    }
}
