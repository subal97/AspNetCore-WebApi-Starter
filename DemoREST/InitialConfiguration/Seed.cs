using DemoREST.Contracts.V1.Requests;
using DemoREST.Data;
using DemoREST.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DemoREST.InitialConfiguration
{
    public static class Seed
    {
        public static async Task AddRoles(IServiceScope scope)
        {
            var _dataContext = new DataContext(scope.ServiceProvider.GetRequiredService<DbContextOptions<DataContext>>());
            await _dataContext.Database.MigrateAsync();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            await AddAdminUserIfNoneFound(scope);
        }

        private static async Task AddAdminUserIfNoneFound(IServiceScope scope)
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var admins = await userManager.GetUsersInRoleAsync("Admin");
            if (admins.Count() > 0)
            {
                return;
            }
            var adminUser = new IdentityUser
            {
                Email = "admin@subal.com",
                UserName = "admin@subal.com",
            };
            var createdUser = await userManager.CreateAsync(adminUser, "Subal@1234");
            if (createdUser.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
