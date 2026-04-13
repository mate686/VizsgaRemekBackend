using Microsoft.AspNetCore.Identity;
using System.Xml.Linq;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.DdSeeders
{
    public class UserSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {

            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();


            if (!await roleManager.RoleExistsAsync("ADMIN"))
            {
                await roleManager.CreateAsync(new IdentityRole("ADMIN"));
            }

            if (!await roleManager.RoleExistsAsync("USER"))
            {
                await roleManager.CreateAsync(new IdentityRole("USER"));
            }


            var adminEmail = "admin@example.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Name = "Admin",
                    Points = 0
                };
                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, "ADMIN");
            }
        }
    }
}
