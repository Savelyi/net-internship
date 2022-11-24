using Data.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using SharedModels.RentEvents;
using SignalR;

namespace CatalogApi.Consumers
{
    public class CarRentedConsumer : IConsumer<ICarRentedEvent>
    {
        private readonly ILogger<CarRentedConsumer> logger;
        private readonly IRepositoryManager repository;
        private readonly IHubContext<CatalogHub, ICatalogHub> hubContext;

        public CarRentedConsumer(ILogger<CarRentedConsumer> logger, IRepositoryManager repository, IHubContext<CatalogHub,ICatalogHub> hubContext)
        {
            this.logger = logger;
            this.repository = repository;
            this.hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<ICarRentedEvent> context)
        {
            var carResult = await repository.Models.GetByIdAsync(context.Message.Id, trackChanges: true);
            if (carResult == null)
            {
                throw new Exception($"Car with Id {context.Message.Id} was not found");
            }

            carResult.IsAvailable = false;
            repository.Models.Update(carResult);
            await repository.SaveAsync();
            await hubContext.Clients.All.CarToRemove();
            logger.LogInformation(
                $"Message received and car with Id {context.Message.Id} is not accepted to be rented or deleted anymore");
        }
    }
}