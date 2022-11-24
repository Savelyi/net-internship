using Microsoft.EntityFrameworkCore;

namespace Data.HangfireContext
{
    public class HangfireDbContext : DbContext
    {
        public HangfireDbContext(DbContextOptions<HangfireDbContext> options)
            : base(options)
        {
        }
    }
}