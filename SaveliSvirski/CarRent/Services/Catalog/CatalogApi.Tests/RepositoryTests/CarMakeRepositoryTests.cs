using Data.CatalogContext;
using Data.Models;
using Data.Repository;
using Data.RequestFeatures;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CatalogApi.Tests.RepositoryTests
{
    public class CarMakeRepositoryTests
    {
        private readonly CatalogDbContext context;
        private readonly CarMakeRepository carMakeRepository;

        public CarMakeRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var contextOptions = new DbContextOptionsBuilder<CatalogDbContext>()
                .UseSqlite(connection)
                .Options;

            context = new CatalogDbContext(contextOptions);

            context.Database.EnsureCreated();
            carMakeRepository = new CarMakeRepository(context);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateNewCarMake()
        {
            //Arrange
            var carMakeId = Guid.NewGuid();
            var testCarMake = GetTestCarMake(carMakeId, "newMake");
            var makesToCheckListBefore = await context.Makes.ToListAsync();
            
            //Act
            await carMakeRepository.CreateAsync(testCarMake);
            await context.SaveChangesAsync();

            //Assert
            var makesToCheckListAfter = await context.Makes.ToListAsync();
            var makeToCheck = makesToCheckListAfter.FirstOrDefault(e => e.Id == carMakeId);
           
            Assert.Equal(makesToCheckListBefore.Count + 1, makesToCheckListAfter.Count);
            Assert.Equal(testCarMake, makeToCheck);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenCarMakeNameAlreadyExists()
        {
            //Arrange
            var carMakeId = Guid.NewGuid();
            var existingMake = (await context.Makes.FirstOrDefaultAsync())?.Make;
            var testCarMake = new CarMake()
            {
                Id = carMakeId,
                Make = existingMake
            };

            //Act
            await carMakeRepository.CreateAsync(testCarMake);
            var act = () => context.SaveChangesAsync();

            //Assert
            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public async Task Delete_ShouldDeleteCarMake()
        {
            //Arrange
            var carMakeId = Guid.NewGuid();
            var testCarMake = GetTestCarMake(carMakeId, "newMake");
            context.Makes.Add(testCarMake);
            await context.SaveChangesAsync();
            var makesToCheckListBefore = context.Makes.ToList();

            //Act
            carMakeRepository.Delete(testCarMake);
            await context.SaveChangesAsync();

            //Assert
            var makesToCheckListAfter = context.Makes.ToList();
            var makeToCheck = makesToCheckListAfter.FirstOrDefault(e => e.Id == carMakeId);
            
            Assert.Equal(makesToCheckListBefore.Count - 1, makesToCheckListAfter.Count);
            Assert.Null(makeToCheck);
        }

        [Theory]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(1, 1)]
        [InlineData(default, default)]
        public async Task GetAllAsync_ShouldGetAllCarMakes_WithPaging(int count, int pageNumber)
        {
            //Arrange
            RequestParameters requestParameters;
            if (count.Equals(default))
            {
                count = context.Makes.Count();
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
            var result = await carMakeRepository.GetAllAsync(requestParameters, default, true);

            //Assert
            Assert.Equal(count, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldGetCertainCarMakeById()
        {
            //Arrange
            var carMakeId = Guid.NewGuid();
            var testCarMake = GetTestCarMake(carMakeId, "newMake");
            context.Makes.Add(testCarMake);
            await context.SaveChangesAsync();

            //Act
            var result = await carMakeRepository.GetByIdAsync(carMakeId, default, true);

            //Assert
            Assert.Equal(testCarMake, result);
        }

        [Fact]
        public async Task Update_ShouldUpdateEntity()
        {
            //Arrange
            var carMakeId = Guid.NewGuid();
            var makeName = "newMake";
            var testCarMake = GetTestCarMake(carMakeId, makeName);
            context.Makes.Add(testCarMake);
            await context.SaveChangesAsync();

            //Act
            testCarMake.Make = "newUpdatedMake";
            carMakeRepository.Update(testCarMake);
            await context.SaveChangesAsync();

            //Assert
            var makeToCheck = await context.Makes.FirstOrDefaultAsync(e => e.Id == carMakeId);
            
            Assert.NotEqual(makeName, makeToCheck.Make);
        }

        [Theory]
        [InlineData("10905a00-6578-41f4-9784-edafbf19ed4b")]
        [InlineData("3c3dbfdc-8329-43a8-aaae-8f2049b564f9")]
        [InlineData("8403feae-5c00-4fab-8ccd-475602fc111e")]
        public async Task GetByConditionAsync_ShouldGetCarMakeByCertainCondition(string carMakeId)
        {
            //Arrange
            var carMakeGuid = Guid.Parse(carMakeId);
            var testCarMake = await context.Makes.FirstOrDefaultAsync(e => e.Id == carMakeGuid);
            
            //Act
            var result =
                await carMakeRepository.GetByConditionAsync(e => e.Id == carMakeGuid, default,
                    true);

            //Assert
            
            Assert.Equal(testCarMake, result);
        }

        [Fact]
        public void GetByCondition_ShouldGetCarMakeByCertainCondition()
        {
            //Arrange
            var carMakeGuidId = new Guid("3c3dbfdc-8329-43a8-aaae-8f2049b564f9");
            var carsToCheckList = context.Makes.Where(e => e.Id == carMakeGuidId);

            //Act
            var result = carMakeRepository.GetByCondition(e => e.Id == carMakeGuidId, default, true);

            //Assert
            Assert.Equal(carsToCheckList, result);
        }

        private CarMake GetTestCarMake(Guid carMakeId, string name)
        {
            return new CarMake()
            {
                Id = carMakeId,
                Make = name
            };
        }

        public void Dispose()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}