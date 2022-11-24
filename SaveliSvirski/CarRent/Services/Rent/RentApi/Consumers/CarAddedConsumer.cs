using Data.Contracts;
using Data.Models;
using MassTransit;
using SharedModels.CatalogEvents;

namespace RentApi.Consumers
{
    public class CarAddedConsumer : IConsumer<ICarAddedEvent>
    {
        private readonly ILogger<CarAddedConsumer> logger;
        private readonly IRepositoryManager repository;

        public CarAddedConsumer(ILogger<CarAddedConsumer> logger, IRepositoryManager repository)
        {
            this.logger = logger;
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<ICarAddedEvent> context)
        {
            var newCarId = context.Message.Id;
            await repository.Cars.CreateAsync(new Car {Id = newCarId});
            await repository.SaveAsync();
            logger.LogInformation($"Message received and car with Id {context.Message.Id} added");
        }
    }
}