using CQRS.Commands;
using CQRS.Handlers.CommandHandlres;
using Data.Contracts;
using Data.Models;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using SharedModels.ErrorModels;
using SignalR;
using Xunit;

namespace CatalogApi.Tests.CQRSHandlersTests.CommandHandlers
{
    public class CarToDeleteHandlerTests
    {
        private readonly Mock<IRepositoryManager> repositoryMock = new Mock<IRepositoryManager>();
        private readonly Mock<IBus> busMock = new Mock<IBus>();
        private readonly Mock<IDistributedCache> cacheMock = new Mock<IDistributedCache>();
        private readonly Mock<IHubContext<CatalogHub, ICatalogHub>> hubContextMock = new Mock<IHubContext<CatalogHub, ICatalogHub>>();

        [Fact]
        public async Task CarToDeleteHandler_ShouldDeleteCarModel_WhenCarExists()
        {
            //Arrange
            var makeId = Guid.NewGuid();
            var testCarModel = GetTestCarModel(makeId);
            var testCarMake = GetTestCarMake(makeId);

            hubContextMock.Setup(e => e.Clients.All.CarToRemove());

            repositoryMock.Setup(r =>
                    r.Models.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(testCarModel);

            repositoryMock.Setup(r => r.Models.Delete(It.IsAny<CarModel>()));
            repositoryMock.Setup(r => r.SaveAsync(It.IsAny<CancellationToken>()));

            repositoryMock.Setup(r =>
                    r.Makes.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(testCarMake);

            var handler = new CarToDeleteHandler(repositoryMock.Object, busMock.Object, cacheMock.Object, hubContextMock.Object);
            var carToDeleteCommand = new CarToDeleteCommand(testCarModel.Id);

            //Act
            await handler.Handle(carToDeleteCommand, default);

            //Assert
            repositoryMock.VerifyAll();
            repositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CarToDeleteHandler_ShouldDeleteCarModel_AndCarMake_WhenCarExists()
        {
            //Arrange
            var makeId = Guid.NewGuid();
            var testCarModel = GetTestCarModel(makeId);

            hubContextMock.Setup(e => e.Clients.All.CarToRemove());

            repositoryMock.Setup(r =>
                    r.Models.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(testCarModel);

            repositoryMock.Setup(r => r.Models.Delete(It.IsAny<CarModel>()));
            repositoryMock.Setup(r => r.SaveAsync(It.IsAny<CancellationToken>()));

            repositoryMock.Setup(r =>
                    r.Makes.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new CarMake()
                {
                    CarModelInfo = new List<CarModel>()
                });

            repositoryMock.Setup(r => r.Makes.Delete(It.IsAny<CarMake>()));

            var handler = new CarToDeleteHandler(repositoryMock.Object, busMock.Object, cacheMock.Object, hubContextMock.Object);
            var carToDeleteCommand = new CarToDeleteCommand(testCarModel.Id);

            //Act
            await handler.Handle(carToDeleteCommand, default);

            //Assert
            repositoryMock.VerifyAll();
            repositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CarToDeleteHandler_ShouldThrowNotFoundException_WhenCarDoesNotExist()
        {
            //Arrange
            repositoryMock.Setup(r =>
                    r.Models.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(() => null);

            var handler = new CarToDeleteHandler(repositoryMock.Object, busMock.Object, cacheMock.Object, hubContextMock.Object);
            var carToDeleteCommand = new CarToDeleteCommand(Guid.NewGuid());

            //Act
            var act = () => handler.Handle(carToDeleteCommand, default);

            //Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
            repositoryMock.VerifyAll();
            repositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CarToDeleteHandler_ShouldThrowRentIssuesException_WhenCarIsNotAvailable()
        {
            //Arrange
            var makeId = Guid.NewGuid();
            var testCarModel = GetTestCarModel(makeId);
            testCarModel.IsAvailable = false;
            repositoryMock.Setup(r =>
                    r.Models.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(testCarModel);

            var handler = new CarToDeleteHandler(repositoryMock.Object, busMock.Object, cacheMock.Object, hubContextMock.Object);
            var carToDeleteCommand = new CarToDeleteCommand(Guid.NewGuid());

            //Act
            var act = () => handler.Handle(carToDeleteCommand, default);

            //Assert
            await Assert.ThrowsAsync<RentIssuesException>(act);
            repositoryMock.VerifyAll();
            repositoryMock.VerifyNoOtherCalls();
        }

        private CarModel GetTestCarModel(Guid makeId)
        {
            return new CarModel()
            {
                CarMakeId = makeId,
                Model = "NewModel",
                RentPrice = 100,
                VehicleNumber = "1337HR-7"
            };
        }

        private CarMake GetTestCarMake(Guid makeId)
        {
            return new CarMake()
            {
                Make = "Make",
                CarModelInfo = new List<CarModel>()
                {
                    GetTestCarModel(makeId)
                }
            };
        }
    }
}