using DemoREST.Data;
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

            if(!await roleManager.RoleExistsAsync("Admin"))
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
