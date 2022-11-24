using System.Reflection;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.CatalogContext
{
    public class CatalogDbContext : DbContext
    {
        public DbSet<CarMake> Makes { get; set; }
        public DbSet<CarModel> Models { get; set; }

        public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
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