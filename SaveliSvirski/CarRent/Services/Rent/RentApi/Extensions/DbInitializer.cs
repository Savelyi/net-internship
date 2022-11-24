using Data.HangfireContext;
using Data.RentContext;
using Microsoft.EntityFrameworkCore;

namespace RentApi.Extensions
{
    public static class DbInitializer
    {
        public static void MigrateDb(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<RentDbContext>())
                {
                    try
                    {
                        appContext.Database.Migrate();
                    }
                    catch (Exception)
                    {
                    }
                }

                using (var hangFireContext = scope.ServiceProvider.GetRequiredService<HangfireDbContext>())
                {
                    hangFireContext.Database.Migrate();
                }
            }
        }
    }
}