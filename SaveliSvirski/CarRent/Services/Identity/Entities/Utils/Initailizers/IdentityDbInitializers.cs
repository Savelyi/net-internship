using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using SharedModels.Constants;

namespace Entities.Utils.Initailizers
{
    public static class IdentityDbInitializers
    {
        public static async Task InitializeRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (await roleManager.FindByNameAsync(UserRoles.User) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }

            if (await roleManager.FindByNameAsync(UserRoles.Admin) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }
        }

        public static async Task InitializeUsersAsync(UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByNameAsync("testAdmin") == null)
            {
                var admin = new IdentityUser
                {
                    UserName = "testAdmin",
                    Email = "testAdmin@gmail.com"
                };
                await userManager.CreateAsync(admin, "testAdminPas");
                await userManager.AddToRoleAsync(admin, "admin");
            }
        }

        public static async Task InitializeConfigurationDbContext(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in IdentityConfig.GetClients())
                {
                    await context.Clients.AddAsync(client.ToEntity());
                }

                await context.SaveChangesAsync();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in IdentityConfig.GetIdentityResources())
                {
                    await context.IdentityResources.AddAsync(resource.ToEntity());
                }

                await context.SaveChangesAsync();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var apiScope in IdentityConfig.GetApiScopes())
                {
                    await context.ApiScopes.AddAsync(apiScope.ToEntity());
                }

                await context.SaveChangesAsync();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in IdentityConfig.GetApiResources())
                {
                    await context.ApiResources.AddAsync(resource.ToEntity());
                }

                await context.SaveChangesAsync();
            }
        }
    }
}