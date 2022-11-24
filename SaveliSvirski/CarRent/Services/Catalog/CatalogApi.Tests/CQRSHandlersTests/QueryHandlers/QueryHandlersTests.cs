using System.Linq.Expressions;
using System.Text.Json;
using AutoMapper;
using BusinessLogic.Dto;
using CQRS.Handlers.QueryHandler;
using CQRS.Queries;
using Data.Contracts;
using Data.Models;
using Data.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using SharedModels.ErrorModels;
using Xunit;

namespace CatalogApi.Tests.CQRSHandlersTests.QueryHandlers
{
    public class QueryHandlersTests
    {
        private readonly Mock<IRepositoryManager> repositoryMock = new Mock<IRepositoryManager>();
        private readonly Mock<IMapper> mapperMock = new Mock<IMapper>();
        private readonly Mock<IDistributedCache> cacheMock = new Mock<IDistributedCache>();

        [Fact]
        public async Task GetCarsHandler_ShouldShowAllCars_WithOutMakeIdFilter()
        {
            //Arrange
            var makeId = Guid.NewGuid();
            var testCarModels = GetListOfTestCarModels(makeId);
            var pagedListCars = GetPagedList(testCarModels);
            var carsToShowDto = GetCarsCarToShowDtos(testCarModels);

            repositoryMock.Setup(r =>
                    r.Models.GetAllAsync(It.IsAny<RequestParameters>(), It.IsAny<CancellationToken>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(pagedListCars);

            mapperMock.Setup(x => x.Map<IEnumerable<CarToShowDto>>(It.IsAny<PagedList<CarModel>>()))
                .Returns(carsToShowDto);

            var handler = new GetCarsHandler(repositoryMock.Object, mapperMock.Object);
            var carsQueryRequest = new GetCarsQuery(default, default);

            //Act
            var result = await handler.Handle(carsQueryRequest, default);

            //Assert
            Assert.Equal(carsToShowDto, result);
            repositoryMock.VerifyAll();
            mapperMock.VerifyAll();
        }


        [Fact]
        public async Task GetCarsHandler_ShouldThrowNotFoundException_WithMakeIdFilter_WhenMakeDoesNotExist()
        {
            //Arrange
            var makeId = Guid.NewGuid();
            repositoryMock.Setup(e =>
                    e.Makes.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(() => null);

            var handler = new GetCarsHandler(repositoryMock.Object, mapperMock.Object);
            var carsQueryRequest = new GetCarsQuery(default, makeId.ToString());

            //Act
            var act = () => handler.Handle(carsQueryRequest, default);

            //Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
            repositoryMock.VerifyAll();
            mapperMock.VerifyAll();
        }

        [Fact]
        public async Task GetCarByIdHandler_ShouldShowCarById_WithOutCache()
        {
            //Arrange
            var testCarModel = GetTestCarModel(Guid.NewGuid(), "TestModel");
            var testCarModelDto = GetCarToShowDto(testCarModel);

            cacheMock.Setup(c => c.Get(It.IsAny<string>()))
                .Returns(() => null);

            repositoryMock.Setup(c =>
                    c.Models.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(testCarModel);

            mapperMock.Setup(x => x.Map<CarToShowDto>(It.IsAny<CarModel>())).Returns(testCarModelDto);

            var handler = new GetCarByIdHandler(repositoryMock.Object, mapperMock.Object, cacheMock.Object);
            var carQueryRequest = new GetCarByIdQuery(testCarModel.Id);

            //Act
            var result = await handler.Handle(carQueryRequest, default);

            //Assert
            Assert.Equal(testCarModelDto, result);
            repositoryMock.VerifyAll();
            cacheMock.VerifyAll();
            mapperMock.VerifyAll();
        }

        [Fact]
        public async Task GetCarByIdHandler_ShouldShowCarById_WithCache()
        {
            //Arrange
            var testCarModel = GetTestCarModel(Guid.NewGuid(), "TestModel");
            var testCarModelDto = GetCarToShowDto(testCarModel);

            cacheMock.Setup(c => c.Get(It.IsAny<string>()))
                .Returns(() => JsonSerializer.SerializeToUtf8Bytes(testCarModelDto));

            var handler = new GetCarByIdHandler(repositoryMock.Object, mapperMock.Object, cacheMock.Object);
            var carQueryRequest = new GetCarByIdQuery(testCarModel.Id);

            //Act
            var result = await handler.Handle(carQueryRequest, default);

            //Assert
            Assert.Equivalent(testCarModelDto, result);
            cacheMock.VerifyAll();
        }

        [Fact]
        public async Task GetCarByIdHandler_ShouldThrowNotFoundException_WhenCarDoesNotExist()
        {
            //Arrange
            cacheMock.Setup(c => c.Get(It.IsAny<string>()))
                .Returns(() => null);

            repositoryMock.Setup(c =>
                    c.Models.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(() => null);

            var handler = new GetCarByIdHandler(repositoryMock.Object, mapperMock.Object, cacheMock.Object);
            var carQueryRequest = new GetCarByIdQuery(Guid.NewGuid());

            //Act
            var act = () => handler.Handle(carQueryRequest, default);

            //Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
            cacheMock.VerifyAll();
            repositoryMock.VerifyAll();
        }

        private PagedList<CarModel> GetPagedList(IEnumerable<CarModel> cars)
        {
            return new PagedList<CarModel>(cars, 1, 1, 10);
        }

        private List<CarToShowDto> GetCarsCarToShowDtos(IEnumerable<CarModel> cars)
        {
            var listToShow = new List<CarToShowDto>();
            foreach (var car in cars)
            {
                listToShow.Add(GetCarToShowDto(car));
            }

            return listToShow;
        }

        private List<CarModel> GetListOfTestCarModels(Guid makeId)
        {
            return new List<CarModel>
            {
                GetTestCarModel(makeId, "TestModel1"),
                GetTestCarModel(makeId, "TestModel2"),
                GetTestCarModel(Guid.NewGuid(), "TestModel3")
            };
        }

        private CarModel GetTestCarModel(Guid makeId, string modelName)
        {
            return new CarModel()
            {
                Model = modelName,
                RentPrice = 100,
                VehicleNumber = "1000HR-7",
                CarMakeId = makeId
            };
        }

        private CarToShowDto GetCarToShowDto(CarModel car)
        {
            return new CarToShowDto()
            {
                Id = car.Id,
                RentPrice = car.RentPrice,
                IsAvailable = car.IsAvailable,
                Make = "SomeMake",
                Model = car.Model,
                VehicleNumber = car.VehicleNumber,
                MakeId = car.CarMakeId
            };
        }
    }
}