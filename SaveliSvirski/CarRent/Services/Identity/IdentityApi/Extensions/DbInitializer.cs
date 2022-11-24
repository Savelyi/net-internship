using Entities;
using Entities.Utils.Initailizers;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityApi.Extensions
{
    public static class DbInitializer
    {
        public static async Task MigrateDbAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                await scope.ServiceProvider.GetRequiredService<RepositoryDbContext>().Database.MigrateAsync();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await IdentityDbInitializers.InitializeRolesAsync(roleManager);
                await IdentityDbInitializers.InitializeUsersAsync(userManager);

                await scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.MigrateAsync();
                using (var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>())
                {
                    await context.Database.MigrateAsync();
                    await IdentityDbInitializers.InitializeConfigurationDbContext(context);
                }
            }
        }
    }
}