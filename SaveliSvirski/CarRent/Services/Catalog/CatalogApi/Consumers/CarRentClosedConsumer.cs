using Data.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using SharedModels.RentEvents;
using SignalR;

namespace CatalogApi.Consumers
{
    public class CarRentClosedConsumer : IConsumer<ICarRentClosedEvent>
    {
        private readonly ILogger<CarRentClosedConsumer> logger;
        private readonly IRepositoryManager repository;
        private readonly IHubContext<CatalogHub, ICatalogHub> hubContext;

        public CarRentClosedConsumer(ILogger<CarRentClosedConsumer> logger, IRepositoryManager repository, IHubContext<CatalogHub, ICatalogHub> hubContext)
        {
            this.logger = logger;
            this.repository = repository;
            this.hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<ICarRentClosedEvent> context)
        {
            var carResult = await repository.Models.GetByIdAsync(context.Message.Id, trackChanges: true);
            if (carResult == null)
            {
                throw new Exception($"Car with Id {context.Message.Id} was not found");
            }

            carResult.IsAvailable = true;
            repository.Models.Update(carResult);
            await repository.SaveAsync();

            await hubContext.Clients.All.CarToAdd();
            logger.LogInformation(
                $"Message received and car with Id {context.Message.Id} is ready to be rented or deleted");
        }
    }
}