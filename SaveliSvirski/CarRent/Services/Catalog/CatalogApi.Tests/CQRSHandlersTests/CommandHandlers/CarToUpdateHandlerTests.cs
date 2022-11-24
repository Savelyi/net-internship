using System.Linq.Expressions;
using AutoMapper;
using CQRS.Commands;
using CQRS.Handlers.CommandHandlres;
using Data.Contracts;
using Data.Models;
using Data.RequestFeatures;
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using SharedModels.ErrorModels;
using Xunit;

namespace CatalogApi.Tests.CQRSHandlersTests.CommandHandlers
{
    public class CarToUpdateHandlerTests
    {
        private readonly Mock<IRepositoryManager> repositoryMock = new Mock<IRepositoryManager>();
        private readonly Mock<IMapper> mapperMock = new Mock<IMapper>();

        private readonly Mock<IValidator<CarToUpdateCommand>> updateValidatorMock =
            new Mock<IValidator<CarToUpdateCommand>>();

        private readonly Mock<IDistributedCache> cacheMock = new Mock<IDistributedCache>();

        [Fact]
        public async Task CarToUpdateHandler_ShouldUpdateCar_AndReturnId_WhenCarMakeExist()
        {
            //Arrange
            var makeId = Guid.NewGuid();
            var carToUpdateCommand = GetTestCarToUpdateCommand();
            var testCarModel = GetTestCarModel(carToUpdateCommand);

            mapperMock.Setup(m => m.Map<CarModel>(It.IsAny<CarToUpdateCommand>()))
                .Returns(testCarModel);

            repositoryMock.Setup(r =>
                    r.Models.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(testCarModel);

            repositoryMock.Setup(r => r.Makes.GetByConditionAsync(It.IsAny<Expression<Func<CarMake, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(GetTestCarMake(makeId));

            repositoryMock.Setup(e => e.Models.Update(It.IsAny<CarModel>()));
            repositoryMock.Setup(e => e.SaveAsync(It.IsAny<CancellationToken>()));

            var handler = new CarToUpdateHandler(mapperMock.Object, repositoryMock.Object, updateValidatorMock.Object,
                cacheMock.Object);

            //Act
            var result = await handler.Handle(carToUpdateCommand, default);

            //Assert
            Assert.Equal(testCarModel.CarMakeId, makeId);
            mapperMock.VerifyAll();
            repositoryMock.VerifyAll();
            repositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CarToUpdateHandler_ShouldUpdateCar_AndReturnId_WhenCarMakeDoesNotExist()
        {
            //Arrange
            var carToUpdateCommand = GetTestCarToUpdateCommand();
            var testCarModel = GetTestCarModel(carToUpdateCommand);

            mapperMock.Setup(m => m.Map<CarModel>(It.IsAny<CarToUpdateCommand>()))
                .Returns(testCarModel);

            repositoryMock.Setup(r =>
                    r.Models.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(testCarModel);

            repositoryMock.Setup(r => r.Makes.GetByConditionAsync(It.IsAny<Expression<Func<CarMake, bool>>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(() => null);

            repositoryMock.Setup(e => e.Models.Update(It.IsAny<CarModel>()));
            repositoryMock.Setup(e => e.Makes.CreateAsync(It.IsAny<CarMake>(), It.IsAny<CancellationToken>()));
            repositoryMock.Setup(e => e.SaveAsync(It.IsAny<CancellationToken>()));

            var handler = new CarToUpdateHandler(mapperMock.Object, repositoryMock.Object, updateValidatorMock.Object,
                cacheMock.Object);

            //Act
            var result = await handler.Handle(carToUpdateCommand, default);

            //Assert
            Assert.NotNull(testCarModel.CarMakeId);
            repositoryMock.VerifyAll();
            repositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CarToUpdateHandler_ShouldThrowNotFoundException_WhenCarModelWasNotFound()
        {
            //Arrange
            var carToUpdateCommand = GetTestCarToUpdateCommand();

            repositoryMock.Setup(r =>
                    r.Models.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(() => null);

            var handler = new CarToUpdateHandler(mapperMock.Object, repositoryMock.Object, updateValidatorMock.Object,
                cacheMock.Object);

            //Act
            var act = () => handler.Handle(carToUpdateCommand, default);

            //Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
            repositoryMock.VerifyAll();
            repositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CarToUpdateHandler_ShouldThrowValidationException_WhenValidationIsFailed()
        {
            //Arrange
            var carToUpdateCommand = GetTestCarToUpdateCommand();

            updateValidatorMock.Setup(v =>
                    v.ValidateAsync(It.IsAny<ValidationContext<CarToUpdateCommand>>(), It.IsAny<CancellationToken>()))
                .Callback(() => throw new ValidationException(""));

            var handler = new CarToUpdateHandler(mapperMock.Object, repositoryMock.Object, updateValidatorMock.Object,
                cacheMock.Object);

            //Act
            var act = () => handler.Handle(carToUpdateCommand, default);

            //Assert
            await Assert.ThrowsAsync<ValidationException>(act);
            updateValidatorMock.VerifyAll();
        }


        private CarMake GetTestCarMake(Guid makeId)
        {
            return new CarMake()
            {
                Id = makeId,
                Make = "Make"
            };
        }

        private CarModel GetTestCarModel(CarToUpdateCommand carToDelete)
        {
            return new CarModel()
            {
                Id = carToDelete.Id,
                Model = carToDelete.Model,
                RentPrice = carToDelete.RentPrice,
                VehicleNumber = carToDelete.VehicleNumber
            };
        }

        private CarToUpdateCommand GetTestCarToUpdateCommand()
        {
            return new CarToUpdateCommand()
            {
                Id = Guid.NewGuid(),
                Model = "newModel",
                RentPrice = 100,
                VehicleNumber = "1000HR-7",
                Make = "Make"
            };
        }
    }
}