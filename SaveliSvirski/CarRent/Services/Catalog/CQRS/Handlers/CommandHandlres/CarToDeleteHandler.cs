using CQRS.Commands;
using Data.Contracts;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using SharedModels.CatalogEvents;
using SharedModels.ErrorModels;
using SignalR;

namespace CQRS.Handlers.CommandHandlres
{
    public class CarToDeleteHandler : IRequestHandler<CarToDeleteCommand, Unit>
    {
        private readonly IRepositoryManager repository;
        private readonly IBus bus;
        private readonly IDistributedCache cache;
        private readonly IHubContext<CatalogHub, ICatalogHub> hubContext;

        public CarToDeleteHandler(IRepositoryManager repository, IBus bus, 
            IDistributedCache cache, IHubContext<CatalogHub, ICatalogHub> hubContext)
        {
            this.repository = repository;
            this.bus = bus;
            this.cache = cache;
            this.hubContext = hubContext;
        }

        public async Task<Unit> Handle(CarToDeleteCommand request, CancellationToken cancellationToken)
        {
            var carModel = await repository.Models.GetByIdAsync(request.Id, cancellationToken, true);
            if (carModel == null)
            {
                throw new NotFoundException($"Car with Id {request.Id} was not found");
            }

            if (carModel.IsAvailable == false)
            {
                throw new RentIssuesException($"Car with Id {request.Id} has been rented, so you can't delete it");
            }

            repository.Models.Delete(carModel);
            await repository.SaveAsync(cancellationToken);
            var carMake = await repository.Makes.GetByIdAsync(carModel.CarMakeId, cancellationToken, true);
            if (carMake.CarModelInfo.Count == 0)
            {
                repository.Makes.Delete(carMake);
                await repository.SaveAsync(cancellationToken);
            }
            
            await cache.RemoveAsync(request.Id.ToString(), cancellationToken);
            await bus.Publish<ICarDeletedEvent>(new CarDeletedEvent
            {
                Id = request.Id
            }, cancellationToken);
            await hubContext.Clients.All.CarToRemove();

            return Unit.Value;
        }
    }
}