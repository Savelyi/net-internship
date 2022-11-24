using System.Linq.Expressions;
using AutoMapper;
using BusinessLogic.Dto;
using BusinessLogic.Services;
using Data.Contracts;
using Data.Models;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SharedModels.Cache;
using SharedModels.ErrorModels;
using Xunit;

namespace RentApi.Tests
{
    public class RentServiceTests
    {
        private readonly Mock<IMapper> mapperMock = new Mock<IMapper>();
        private readonly Mock<IRepositoryManager> repositoryMock = new Mock<IRepositoryManager>();
        private readonly Mock<IBus> busMock = new Mock<IBus>();
        private readonly Mock<ILogger<RentService>> loggerMock = new Mock<ILogger<RentService>>();
        private readonly Mock<IDistributedCache> cacheMock = new Mock<IDistributedCache>();
        private readonly Mock<IOptions<RedisOptions>> redisOptionsMock = new Mock<IOptions<RedisOptions>>();
        private readonly RentService service;

        public RentServiceTests()
        {
            service = new RentService(mapperMock.Object, repositoryMock.Object,
                busMock.Object, loggerMock.Object,
                cacheMock.Object, redisOptionsMock.Object);
        }

        [Fact]
        public async Task ShowUserRentById_ShouldShowUserRent_WhenRentExists()
        {
            //Arrange
            var testRent = GetTestRent();

            mapperMock.Setup(x => x.Map<RentToShowDto>(It.IsAny<Rent>()))
                .Returns(new RentToShowDto()
                {
                    Id = testRent.Id,
                    CarId = testRent.CarId,
                    Closed = testRent.Closed,
                    Created = testRent.Created,
                    IsClosed = testRent.IsClosed
                });

            repositoryMock.Setup(x =>
                    x.Rents.GetByIdAsync(testRent.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(testRent);

            //Act
            var result = await service.ShowUserRentAsync(testRent.Id, testRent.UserId, new CancellationToken());

            //Assert
            Assert.Equal(testRent.Id, result.Id);
            repositoryMock.VerifyAll();
            mapperMock.VerifyAll();
        }

        [Fact]
        public async Task ShowUserRentById_ShouldThrowException_WhenRentDoesNotExist()
        {
            //Arrange
            var testRent = GetTestRent();

            repositoryMock.Setup(x =>
                    x.Rents.GetByIdAsync(testRent.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(() => null);

            //Act
            var act = () => service.ShowUserRentAsync(testRent.Id, testRent.UserId, new CancellationToken());

            //Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
            repositoryMock.VerifyAll();
        }

        [Fact]
        public async Task CreateUserRentAsync_ShouldCreateRent_WhenCarExists()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var testCar = new Car();
            
            repositoryMock.Setup(r => r.Cars.GetByIdAsync(testCar.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(testCar);
            repositoryMock.Setup(r => r.Rents.CreateAsync(It.IsAny<Rent>(), It.IsAny<CancellationToken>()));

            //Act
            await service.CreateRentAsync(testCar.Id, userId, default);

            //Assert
            Assert.False(testCar.IsAvailable);
            repositoryMock.VerifyAll();
        }

        [Fact]
        public async Task CreateUserRentAsync_ShouldCreateRent_WhenCarIsNotFound()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var testCar = new Car();
            
            repositoryMock.Setup(r => r.Cars.GetByIdAsync(testCar.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(() => null);

            //Act
            var act = () => service.CreateRentAsync(testCar.Id, userId, default);

            //Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
            repositoryMock.VerifyAll();
        }

        [Fact]
        public async Task CloseRentAsync_ShouldCloseRent_WhenRentIsNotClosed()
        {
            //Arrange
            var testRent = GetTestRent();
            testRent.CarInfo = new Car();
            
            repositoryMock.Setup(x =>
                    x.Rents.GetByIdAsync(testRent.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(testRent);
            repositoryMock.Setup(r => r.SaveAsync(It.IsAny<CancellationToken>()));

            //Act
            await service.CloseRentAsync(testRent.Id, testRent.UserId, default);

            //Assert
            Assert.True(testRent.IsClosed);
            Assert.True(testRent.CarInfo.IsAvailable);
            repositoryMock.VerifyAll();
        }

        [Fact]
        public async Task CloseRentAsync_ShouldThrowException_WhenRentIsClosed()
        {
            //Arrange
            var testRent = GetTestRent();
            testRent.CarInfo = new Car();
            testRent.IsClosed = true;
            
            repositoryMock.Setup(x =>
                    x.Rents.GetByIdAsync(testRent.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(testRent);

            //Act
            var act = () => service.CloseRentAsync(testRent.Id, testRent.UserId, default);

            //Assert
            await Assert.ThrowsAsync<RentIssuesException>(act);
        }

        [Fact]
        public async Task DeleteRentAsync_ShouldDeleteRent_WhenRentIsClosed()
        {
            //Arrange
            var testRent = GetTestRent();
            testRent.IsClosed = true;
            
            repositoryMock.Setup(x =>
                    x.Rents.GetByIdAsync(testRent.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(testRent);
            repositoryMock.Setup(r => r.SaveAsync(It.IsAny<CancellationToken>()));

            //Act
            await service.DeleteRentAsync(testRent.Id, testRent.UserId, default);

            //Assert
            repositoryMock.VerifyAll();
        }

        [Fact]
        public async Task DeleteRentAsync_ShouldThrowException_WhenRentIsNotClosed()
        {
            //Arrange
            var testRent = GetTestRent();
            
            repositoryMock.Setup(x =>
                    x.Rents.GetByIdAsync(testRent.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(testRent);

            //Act
            var act = () => service.DeleteRentAsync(testRent.Id, testRent.UserId, default);

            //Assert
            await Assert.ThrowsAsync<RentIssuesException>(act);
            Assert.False(testRent.IsClosed);
            repositoryMock.VerifyAll();
        }

        [Fact]
        public async Task DeleteRentForAdminAsync_ShouldDeleteRent_WhenRentIsClosed()
        {
            //Arrange
            var testRent = GetTestRent();
            testRent.IsClosed = true;
            
            repositoryMock.Setup(x =>
                    x.Rents.GetByIdAsync(testRent.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(testRent);
            repositoryMock.Setup(r => r.SaveAsync(It.IsAny<CancellationToken>()));

            //Act
            await service.DeleteRentForAdminAsync(testRent.Id, default);

            //Assert
            repositoryMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()));
            repositoryMock.VerifyAll();
        }

        [Fact]
        public async Task DeleteRentForAdminAsync_ShouldThrowException_WhenRentIsNotClosed()
        {
            //Arrange
            var testRent = GetTestRent();
            
            repositoryMock.Setup(x =>
                    x.Rents.GetByIdAsync(testRent.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(testRent);

            //Act
            var act = () => service.DeleteRentForAdminAsync(testRent.Id, default);

            //Assert
            await Assert.ThrowsAsync<RentIssuesException>(act);
            Assert.False(testRent.IsClosed);
            repositoryMock.VerifyAll();
        }

        [Fact]
        public async Task DeleteRentForAdminAsync_ShouldThrowException_WhenRentIsNotFound()
        {
            //Arrange
            var testRent = GetTestRent();
            
            repositoryMock.Setup(x =>
                    x.Rents.GetByIdAsync(testRent.Id, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(() => null);

            //Act
            var act = () => service.DeleteRentForAdminAsync(testRent.Id, default);

            //Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.False(testRent.IsClosed);
            repositoryMock.VerifyAll();
        }

        [Fact]
        public async Task DoSomeProcessWithLock_ImitationOfWorkWithRedLock()
        {
            //Arrange
            redisOptionsMock.Setup(s => s.Value).Returns(new RedisOptions
            {
                Host = "SomeHost",
                Port = 1337
            });
            //Act
            await service.DoSomeProcessWithLockAsync();
            //Assert
            redisOptionsMock.VerifyAll();
        }

        private Rent GetTestRent()
        {
            var userId = Guid.NewGuid();
            var carId = Guid.NewGuid();
            return new Rent
            {
                UserId = userId,
                CarId = carId
            };
        }
    }
}