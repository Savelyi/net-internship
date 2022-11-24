using Data.CatalogContext;
using Data.Models;
using Data.Repository;
using Data.RequestFeatures;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CatalogApi.Tests.RepositoryTests
{
    public class CarModelRepositoryTests
    {
        private readonly CatalogDbContext context;
        private readonly CarModelRepository carModelRepository;

        public CarModelRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var contextOptions = new DbContextOptionsBuilder<CatalogDbContext>()
                .UseSqlite(connection)
                .Options;

            context = new CatalogDbContext(contextOptions);

            context.Database.EnsureCreated();
            carModelRepository = new CarModelRepository(context);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateNewCarModel_WhenCarMakeExists()
        {
            //Arrange
            var existingCarMakeId = new Guid("10905a00-6578-41f4-9784-edafbf19ed4b");
            var carModelId = Guid.NewGuid();
            var testCarModel = GetTestCarModel(carModelId, existingCarMakeId);
            var carsToCheckListBefore = await context.Models.ToListAsync();

            //Act
            await carModelRepository.CreateAsync(testCarModel);
            await context.SaveChangesAsync();

            //Assert
            var carsToCheckListAfter = await context.Models.ToListAsync();
            var carModelToCheck = carsToCheckListAfter.FirstOrDefault(e => e.Id == carModelId);
            
            Assert.Equal(carsToCheckListBefore.Count + 1, carsToCheckListAfter.Count);
            Assert.Equal(carModelToCheck, testCarModel);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenCarMakeDoesNotExist()
        {
            //Arrange
            var carModelId = Guid.NewGuid();
            var testCarModel = GetTestCarModel(carModelId, Guid.NewGuid());

            //Act
            await carModelRepository.CreateAsync(testCarModel);
            var act = () => context.SaveChangesAsync();

            //Assert
            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public async Task Delete_ShouldDeleteCarModel()
        {
            //Arrange
            var carModelId = Guid.NewGuid();
            var existingCarMakeId = new Guid("10905a00-6578-41f4-9784-edafbf19ed4b");
            var testCarModel = GetTestCarModel(carModelId, existingCarMakeId);
            context.Models.Add(testCarModel);
            await context.SaveChangesAsync();
            var carsToCheckListBefore = context.Models.ToList();

            //Act
            carModelRepository.Delete(testCarModel);
            await context.SaveChangesAsync();

            //Assert
            var carsToCheckListAfter = context.Models.ToList();
            var carToCheck = carsToCheckListAfter.FirstOrDefault(e => e.Id == carModelId);
            
            Assert.Equal(carsToCheckListBefore.Count - 1, carsToCheckListAfter.Count);
            Assert.Null(carToCheck);
        }

        [Theory]
        [InlineData(5, 1)]
        [InlineData(3, 1)]
        [InlineData(6, 1)]
        [InlineData(default, default)]
        public async Task GetAllAsync_ShouldGetAllCarModels_WithPaging(int count, int pageNumber)
        {
            //Arrange
            RequestParameters requestParameters;
            if (count.Equals(default))
            {
                count = context.Models.Count();
                requestParameters = new RequestParameters();
            }
            else
            {
                requestParameters = new RequestParameters()
                {
                    PageSize = count,
                    PageNumber = pageNumber
                };
            }

            //Act
            var result = await carModelRepository.GetAllAsync(requestParameters, default, true);

            //Assert
            Assert.Equal(count, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldGetCertainCarModelById()
        {
            //Arrange
            var carModelId = Guid.NewGuid();
            var existingCarMakeId = new Guid("10905a00-6578-41f4-9784-edafbf19ed4b");
            var testCarModel = GetTestCarModel(carModelId, existingCarMakeId);
            context.Models.Add(testCarModel);
            await context.SaveChangesAsync();

            //Act
            var result = await carModelRepository.GetByIdAsync(carModelId, default, true);

            //Assert
            Assert.Equal(testCarModel, result);
        }

        [Fact]
        public async Task Update_ShouldUpdateEntity()
        {
            //Arrange
            var carModelId = Guid.NewGuid();
            var existingCarMakeId = new Guid("10905a00-6578-41f4-9784-edafbf19ed4b");
            var testCarModel = GetTestCarModel(carModelId, existingCarMakeId);
            var rentPrice = testCarModel.RentPrice;
            context.Models.Add(testCarModel);
            await context.SaveChangesAsync();

            //Act
            testCarModel.RentPrice = 250;
            carModelRepository.Update(testCarModel);
            await context.SaveChangesAsync();

            //Assert
            var carToCheck = await context.Models.FirstOrDefaultAsync(e => e.Id == carModelId);
            
            Assert.NotEqual(rentPrice, carToCheck.RentPrice);
        }

        [Theory]
        [InlineData("10905a00-6578-41f4-9784-edafbf19ed4b")]
        [InlineData("3c3dbfdc-8329-43a8-aaae-8f2049b564f9")]
        [InlineData("8403feae-5c00-4fab-8ccd-475602fc111e")]
        public async Task GetByConditionAsync_ShouldGetCarModelByCertainCondition(string carMakeId)
        {
            //Arrange
            var carMakeGuid = Guid.Parse(carMakeId);
            var testCarModel = await context.Models.FirstOrDefaultAsync(e => e.Id == carMakeGuid);

            //Act
            var result =
                await carModelRepository.GetByConditionAsync(e => e.Id == carMakeGuid, default,
                    true);

            //Assert

            Assert.Equal(testCarModel, result);
        }

        [Fact]
        public void GetByCondition_ShouldGetCarModelByCertainCondition()
        {
            //Arrange
            var carMakeGuidId = new Guid("3c3dbfdc-8329-43a8-aaae-8f2049b564f9");
            var carsToCheckList = context.Models.Where(e => e.CarMakeId == carMakeGuidId);

            //Act
            var result = carModelRepository.GetByCondition(e => e.CarMakeId == carMakeGuidId, default, true);

            //Assert
            Assert.Equal(carsToCheckList, result);
        }

        private CarModel GetTestCarModel(Guid carModelId, Guid existingCarMakeId)
        {
            return new CarModel()
            {
                Id = carModelId,
                CarMakeId = existingCarMakeId,
                RentPrice = 100,
                Model = "newTestModel",
                VehicleNumber = "1337HR-7"
            };
        }

        public void Dispose()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}