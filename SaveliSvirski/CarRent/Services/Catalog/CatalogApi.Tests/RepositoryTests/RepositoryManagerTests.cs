using Data.CatalogContext;
using Data.Contracts;
using Data.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CatalogApi.Tests.RepositoryTests
{
    public class RepositoryManagerTests
    {
        private readonly CatalogDbContext context;
        private IRepositoryManager manager;

        public RepositoryManagerTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var contextOptions = new DbContextOptionsBuilder<CatalogDbContext>()
                .UseSqlite(connection)
                .Options;

            context = new CatalogDbContext(contextOptions);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [Fact]
        public void GetCarModelRepository()
        {
            //Arrange
            manager = new RepositoryManager(context);

            //Act
            var result = manager.Models;

            //Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<ICarModelRepository>(result);
        }

        [Fact]
        public void GetCarMakeRepository()
        {
            //Arrange
            manager = new RepositoryManager(context);

            //Act
            var result = manager.Makes;

            //Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<ICarMakeRepository>(result);
        }
    }
}