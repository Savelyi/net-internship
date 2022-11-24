using Data.CatalogContext;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Extensions
{
    public static class DbInitializer
    {
        public static void MigrateDb(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>())
                {
                    try
                    {
                        appContext.Database.Migrate();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}