using Data.Models;
using Data.RentContext;
using Data.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace RentApi.Tests.RepositoryTests
{
    public class CarRepositoryTests
    {
        private readonly RentDbContext context;

        public CarRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var contextOptions = new DbContextOptionsBuilder<RentDbContext>()
                .UseSqlite(connection)
                .Options;

            context = new RentDbContext(contextOptions);

            context.Database.EnsureCreated();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateNewCar()
        {
            //Arrange
            var carRepository = new CarRepository(context);
            var carId = Guid.NewGuid();
            var testCar = new Car
            {
                Id = carId
            };

            //Act
            await carRepository.CreateAsync(testCar);
            await context.SaveChangesAsync();

            //Assert
            var carToCheck = context.Cars.FirstOrDefault(c => c.Id == carId);
            
            Assert.NotNull(carToCheck);
            Assert.Null(carToCheck.RentInfo);
            Assert.Equal(carToCheck.Id, carId);
        }


        [Fact]
        public async Task Delete_ShouldDeleteCar()
        {
            //Arrange
            var carRepository = new CarRepository(context);
            var carId = Guid.NewGuid();
            var testCar = new Car
            {
                Id = carId
            };
            context.Cars.Add(testCar);
            await context.SaveChangesAsync();
            var carsToCheckBefore = context.Cars.ToList();

            //Act
            carRepository.Delete(testCar);
            await context.SaveChangesAsync();

            //Assert
            var carsToCheckAfter = context.Cars.ToList();
            var deletedCar = context.Cars.FirstOrDefault(c => c.Id == carId);
            
            Assert.Equal(carsToCheckAfter.Count, carsToCheckBefore.Count - 1);
            Assert.Null(deletedCar);
        }

        [Fact]
        public void GetAll_ShouldGetAllCars()
        {
            //Arrange
            var carRepository = new CarRepository(context);
            var rentsToCheckBefore = context.Cars.ToList();

            //Act
            var result = carRepository.GetAll(true);

            //Assert
            Assert.Equal(rentsToCheckBefore, result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldGetCertainRentById()
        {
            //Arrange
            var carRepository = new CarRepository(context);
            var carId = new Guid("eff31885-c7d4-4f88-8559-4a676fdc5a5c");
            var carToCompareWith = await context.Cars.FirstOrDefaultAsync(c => c.Id == carId);

            //Act
            var result = await carRepository.GetByIdAsync(carId, trackChanges: true);

            //Assert
            Assert.Equal(carToCompareWith, result);
        }

        [Fact]
        public async Task Update_ShouldUpdateEntity()
        {
            //Arrange
            var carRepository = new CarRepository(context);
            var carId = new Guid("eff31885-c7d4-4f88-8559-4a676fdc5a5c");
            var carToUpdate = context.Cars.FirstOrDefault(c => c.Id == carId);

            //Act
            carToUpdate.IsAvailable = false;
            carRepository.Update(carToUpdate);
            await context.SaveChangesAsync();

            //Assert
            var carToCheck = context.Cars.FirstOrDefault(c => c.Id == carId);
            
            Assert.NotNull(carToCheck);
            Assert.Equal(carToUpdate.Id, carToCheck.Id);
            Assert.False(carToCheck.IsAvailable);
        }

        public void Dispose()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}