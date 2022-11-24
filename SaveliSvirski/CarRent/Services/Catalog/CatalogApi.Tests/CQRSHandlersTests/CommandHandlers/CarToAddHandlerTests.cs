using System.Linq.Expressions;
using AutoMapper;
using CQRS.Commands;
using CQRS.Handlers.CommandHandlres;
using Data.Contracts;
using Data.Models;
using Data.RequestFeatures;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Moq;
using SignalR;
using Xunit;

namespace CatalogApi.Tests.CQRSHandlersTests.CommandHandlers
{
    public class CarToAddHandlerTests
    {
        private readonly Mock<IRepositoryManager> repositoryMock = new Mock<IRepositoryManager>();
        private readonly Mock<IMapper> mapperMock = new Mock<IMapper>();
        private readonly Mock<IValidator<CarToAddCommand>> addValidatorMock = new Mock<IValidator<CarToAddCommand>>();
        private readonly Mock<IBus> busMock = new Mock<IBus>();
        private readonly Mock<IHubContext<CatalogHub,ICatalogHub>> hubContextMock = new Mock<IHubContext<CatalogHub,ICatalogHub>>();

        [Fact]
        public async Task CarToAddHandler_ShouldCreateCar_AndReturnId_WhenCarMakeExist()
        {
            //Arrange
            var makeId = Guid.NewGuid();
            var carToAddCommand = GetTestCarToAddCommand();
            var testCarModel = GetTestCarModel(carToAddCommand);

            hubContextMock.Setup(e => e.Clients.All.CarToAdd());

            mapperMock.Setup(m => m.Map<CarModel>(It.IsAny<CarToAddCommand>()))
                .Returns(testCarModel);

            repositoryMock.Setup(r => r.Makes.GetByConditionAsync(It.IsAny<Expression<Func<CarMake, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(GetTestCarMake(makeId));

            repositoryMock.Setup(e => e.Models.CreateAsync(It.IsAny<CarModel>(), It.IsAny<CancellationToken>()));
            repositoryMock.Setup(e => e.SaveAsync(It.IsAny<CancellationToken>()));

            var handler = new CarToAddHandler(mapperMock.Object, repositoryMock.Object, addValidatorMock.Object,
                busMock.Object,hubContextMock.Object);

            //Act
            var result = await handler.Handle(carToAddCommand, default);

            //Assert
            Assert.Equal(testCarModel.CarMakeId, makeId);
            repositoryMock.VerifyAll();
            repositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CarToAddHandler_ShouldCreateCar_AndReturnId_WhenCarMakeDoesNotExist()
        {
            //Arrange
            var carToAddCommand = GetTestCarToAddCommand();
            var testCarModel = GetTestCarModel(carToAddCommand);

            hubContextMock.Setup(e => e.Clients.All.CarToAdd());

            mapperMock.Setup(m => m.Map<CarModel>(It.IsAny<CarToAddCommand>()))
                .Returns(testCarModel);

            repositoryMock.Setup(r => r.Makes.GetByConditionAsync(It.IsAny<Expression<Func<CarMake, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(()=>null);


            repositoryMock.Setup(e => e.Models.CreateAsync(It.IsAny<CarModel>(), It.IsAny<CancellationToken>()));
            repositoryMock.Setup(e => e.Makes.CreateAsync(It.IsAny<CarMake>(), It.IsAny<CancellationToken>()));
            repositoryMock.Setup(e => e.SaveAsync(It.IsAny<CancellationToken>()));

            var handler = new CarToAddHandler(mapperMock.Object, repositoryMock.Object, addValidatorMock.Object,
                busMock.Object, hubContextMock.Object);

            //Act
            var result = await handler.Handle(carToAddCommand, default);

            //Assert
            Assert.NotNull(testCarModel.CarMakeId);
            repositoryMock.VerifyAll();
            repositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CarToAddHandler_ShouldThrowValidationException_WhenValidationIsFailed()
        {
            //Arrange
            var carToAddCommand = GetTestCarToAddCommand();

            addValidatorMock.Setup(v =>
                    v.ValidateAsync(It.IsAny<ValidationContext<CarToAddCommand>>(), It.IsAny<CancellationToken>()))
                .Callback(() => throw new ValidationException(""));

            var handler = new CarToAddHandler(mapperMock.Object, repositoryMock.Object, addValidatorMock.Object,
                busMock.Object, hubContextMock.Object);

            //Act
            var act = () => handler.Handle(carToAddCommand, default);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(act);
            addValidatorMock.VerifyAll();
        }


        private CarMake GetTestCarMake(Guid makeId)
        {
            return new CarMake()
            {
                Id = makeId,
                Make = "Make"
            };
        }

        private CarModel GetTestCarModel(CarToAddCommand carToAdd)
        {
            return new CarModel()
            {
                Model = carToAdd.Model,
                RentPrice = carToAdd.RentPrice,
                VehicleNumber = carToAdd.VehicleNumber
            };
        }

        private CarToAddCommand GetTestCarToAddCommand()
        {
            return new CarToAddCommand()
            {
                Model = "newModel",
                RentPrice = 100,
                VehicleNumber = "1000HR-7",
                Make = "Make"
            };
        }
    }
}