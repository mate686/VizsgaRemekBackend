using Microsoft.AspNetCore.Identity;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Data
{
    public class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();


            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {   
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }
    }
}
