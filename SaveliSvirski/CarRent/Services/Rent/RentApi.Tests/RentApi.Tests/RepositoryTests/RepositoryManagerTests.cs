using Data.Contracts;
using Data.RentContext;
using Data.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace RentApi.Tests.RepositoryTests
{
    public class RepositoryManagerTests
    {
        private readonly RentDbContext context;
        private IRepositoryManager manager;

        public RepositoryManagerTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var contextOptions = new DbContextOptionsBuilder<RentDbContext>()
                .UseSqlite(connection)
                .Options;

            context = new RentDbContext(contextOptions);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [Fact]
        public void GetCarRepository()
        {
            //Arrange
            manager = new RepositoryManager(context);

            //Act
            var result = manager.Cars;

            //Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<ICarRepository>(result);
        }

        [Fact]
        public void GetRentRepository()
        {
            //Arrange
            manager = new RepositoryManager(context);

            //Act
            var result = manager.Rents;

            //Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IRentRepository>(result);
        }
    }
}