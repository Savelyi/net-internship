using System.Reflection;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.RentContext
{
    public class RentDbContext : DbContext
    {
        public DbSet<Rent> Rents { get; set; }
        public DbSet<Car> Cars { get; set; }

        public RentDbContext(DbContextOptions<RentDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}