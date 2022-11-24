using Data.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedModels.CatalogEvents;
using SharedModels.ErrorModels;

namespace RentApi.Consumers
{
    public class CarDeletedConsumer : IConsumer<ICarDeletedEvent>
    {
        private readonly IRepositoryManager repository;
        private readonly ILogger<CarDeletedConsumer> logger;

        public CarDeletedConsumer(IRepositoryManager repository, ILogger<CarDeletedConsumer> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<ICarDeletedEvent> context)
        {
            var result = await repository.Rents.GetByCondition(e => e.CarId == context.Message.Id, true)
                .FirstOrDefaultAsync();
            if (result != null)
            {
                repository.Rents.Delete(result);
            }

            var carToDelete = await repository.Cars.GetByIdAsync(context.Message.Id, default, true);
            if (carToDelete == null)
            {
                throw new NotFoundException($"Car with Id {context.Message.Id} was not found");
            }

            repository.Cars.Delete(carToDelete);
            await repository.SaveAsync();
            logger.LogInformation($"Message received and car with Id {context.Message.Id} deleted");
        }
    }
}