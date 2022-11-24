using Data.Models;
using Data.RentContext;
using Data.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace RentApi.Tests.RepositoryTests
{
    public class RentRepositoryTests
    {
        private readonly RentDbContext context;
        private readonly RentRepository rentRepository;

        public RentRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var contextOptions = new DbContextOptionsBuilder<RentDbContext>()
                .UseSqlite(connection)
                .Options;

            context = new RentDbContext(contextOptions);

            context.Database.EnsureCreated();
            rentRepository = new RentRepository(context);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateNewRent_WhenCarExists()
        {
            //Arrange
            var existingCarId = new Guid("eff31885-c7d4-4f88-8559-4a676fdc5a5c");
            var rentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var testRent = new Rent
            {
                Id = rentId,
                CarId = existingCarId,
                UserId = userId,
                Created = DateTime.Now
            };

            //Act
            await rentRepository.CreateAsync(testRent);
            await context.SaveChangesAsync();

            //Assert
            var rentsToCheckList = context.Rents.Include(c => c.CarInfo).ToList();
            var rentToCheck = rentsToCheckList.FirstOrDefault();

            Assert.Single(rentsToCheckList);
            Assert.NotNull(rentToCheck);
            Assert.NotNull(rentToCheck.CarInfo);
            Assert.Equal(rentToCheck.UserId, userId);
            Assert.Equal(rentToCheck.Id, rentId);
            Assert.Equal(rentToCheck.CarId, existingCarId);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenCarDoesNotExist()
        {
            //Act
            await rentRepository.CreateAsync(new Rent());
            var act = () => context.SaveChangesAsync();

            //Assert
            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public async Task Delete_ShouldDeleteRent()
        {
            //Arrange
            var carId = new Guid("eff31885-c7d4-4f88-8559-4a676fdc5a5c");
            var rentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var testRent = new Rent
            {
                Id = rentId,
                CarId = carId,
                UserId = userId
            };
            context.Rents.Add(testRent);
            await context.SaveChangesAsync();
            var rentsToCheckBefore = context.Rents.ToList();

            //Act
            rentRepository.Delete(testRent);
            await context.SaveChangesAsync();

            //Assert
            var rentsToCheckAfter = context.Rents.ToList();
            
            Assert.Single(rentsToCheckBefore);
            Assert.Empty(rentsToCheckAfter);
        }

        [Fact]
        public async Task GetAll_ShouldGetAllRents()
        {
            //Arrange
            context.Rents.AddRange(new Rent
                {
                    Id = Guid.NewGuid(),
                    CarId = new Guid("eff31885-c7d4-4f88-8559-4a676fdc5a5c"),
                    UserId = Guid.NewGuid()
                },
                new Rent
                {
                    Id = Guid.NewGuid(),
                    CarId = new Guid("1e8743d3-bf6b-4bfd-8b74-3d523c8042cd"),
                    UserId = Guid.NewGuid()
                });
            await context.SaveChangesAsync();
            var rentsToCheckBefore = await context.Rents.ToListAsync();

            //Act
            var result = rentRepository.GetAll(true);

            //Assert
            Assert.Equal(rentsToCheckBefore, result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldGetCertainRentById()
        {
            //Arrange
            var carId = new Guid("eff31885-c7d4-4f88-8559-4a676fdc5a5c");
            var rentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var testRent = new Rent
            {
                Id = rentId,
                CarId = carId,
                UserId = userId
            };
            context.Rents.Add(testRent);
            await context.SaveChangesAsync();

            //Act
            var result = await rentRepository.GetByIdAsync(rentId, trackChanges: true);

            //Assert
            Assert.Equal(testRent.Id, result.Id);
            Assert.Equal(testRent.CarId, result.CarId);
            Assert.Equal(testRent.Closed, result.Closed);
            Assert.Equal(testRent.Created, result.Created);
            Assert.Equal(testRent.UserId, result.UserId);
            Assert.Equal(testRent.IsClosed, result.IsClosed);
        }

        [Fact]
        public async Task Update_ShouldUpdateEntity()
        {
            //Arrange
            var carId = new Guid("eff31885-c7d4-4f88-8559-4a676fdc5a5c");
            var rentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var testRent = new Rent
            {
                Id = rentId,
                CarId = carId,
                UserId = userId
            };
            context.Rents.Add(testRent);
            await context.SaveChangesAsync();

            //Act
            testRent.IsClosed = true;
            testRent.Closed = DateTime.Now;
            rentRepository.Update(testRent);
            await context.SaveChangesAsync();

            //Assert
            var rentsToCheckList = context.Rents.ToList();
            var rentToCheck = rentsToCheckList.FirstOrDefault();
            
            Assert.Single(rentsToCheckList);
            Assert.Equal(testRent.Id, rentToCheck.Id);
            Assert.Equal(testRent.CarId, rentToCheck.CarId);
            Assert.Equal(testRent.Closed, rentToCheck.Closed);
            Assert.Equal(testRent.Created, rentToCheck.Created);
            Assert.Equal(testRent.UserId, rentToCheck.UserId);
            Assert.True(rentToCheck.IsClosed);
        }

        [Fact]
        public async Task GetByCondition_ShouldGetRentByCertainCondition()
        {
            //Arrange
            var carId = new Guid("eff31885-c7d4-4f88-8559-4a676fdc5a5c");
            var rentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var testRent = new Rent
            {
                Id = rentId,
                CarId = carId,
                UserId = userId
            };
            context.Rents.Add(testRent);
            await context.SaveChangesAsync();

            //Act
            var result = rentRepository.GetByCondition(r => r.CarId == carId);

            //Assert
            var rentToCheck = result.FirstOrDefault();
            
            Assert.Single(result);
            Assert.Equal(testRent.Id, rentToCheck.Id);
            Assert.Equal(testRent.CarId, rentToCheck.CarId);
            Assert.Equal(testRent.IsClosed, rentToCheck.IsClosed);
            Assert.Equal(testRent.Closed, rentToCheck.Closed);
            Assert.Equal(testRent.Created, rentToCheck.Created);
            Assert.Equal(testRent.UserId, rentToCheck.UserId);
        }

        public void Dispose()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}