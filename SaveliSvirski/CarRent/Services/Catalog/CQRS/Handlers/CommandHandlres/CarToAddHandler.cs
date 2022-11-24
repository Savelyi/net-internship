using AutoMapper;
using CQRS.Commands;
using Data.Contracts;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using SharedModels.CatalogEvents;
using SignalR;

namespace CQRS.Handlers.CommandHandlres
{
    public class CarToAddHandler : CarToManipulateBaseHandler<CarToAddCommand, Guid>
    {
        private readonly IBus bus;
        private readonly IHubContext<CatalogHub, ICatalogHub> hubContext;

        public CarToAddHandler(IMapper mapper, IRepositoryManager repository, IValidator<CarToAddCommand> validator,
            IBus bus, IHubContext<CatalogHub,ICatalogHub> hubContext)
            : base(mapper, repository, validator)
        {
            this.bus = bus;
            this.hubContext = hubContext;
        }

        public override async Task<Guid> Handle(CarToAddCommand request, CancellationToken cancellationToken)
        {
            await validator.ValidateAndThrowAsync(request);
            var carModel = await CreateMakeIfNotExistAsync(request, cancellationToken);
            await repository.Models.CreateAsync(carModel, cancellationToken);
            await repository.SaveAsync(cancellationToken);
            
            await bus.Publish<ICarAddedEvent>(new CarAddedEvent
            {
                Id = carModel.Id
            }, cancellationToken);
            await hubContext.Clients.All.CarToAdd();

            return carModel.Id;
        }
    }
}