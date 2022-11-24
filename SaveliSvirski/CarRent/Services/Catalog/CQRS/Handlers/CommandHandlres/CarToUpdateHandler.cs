using AutoMapper;
using CQRS.Commands;
using Data.Contracts;
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;
using SharedModels.ErrorModels;

namespace CQRS.Handlers.CommandHandlres
{
    public class CarToUpdateHandler : CarToManipulateBaseHandler<CarToUpdateCommand, Guid>
    {
        private readonly IDistributedCache cache;

        public CarToUpdateHandler(IMapper mapper, IRepositoryManager repository,
            IValidator<CarToUpdateCommand> validator, IDistributedCache cache)
            : base(mapper, repository, validator)
        {
            this.cache = cache;
        }

        public override async Task<Guid> Handle(CarToUpdateCommand request, CancellationToken cancellationToken)
        {
            await validator.ValidateAndThrowAsync(request);
            if (await repository.Models.GetByIdAsync(request.Id, cancellationToken, false) == null)
            {
                throw new NotFoundException($"Car with Id {request.Id} was not found");
            }

            var carModel = await CreateMakeIfNotExistAsync(request, cancellationToken);
            repository.Models.Update(carModel);
            await repository.SaveAsync(cancellationToken);
            await cache.RemoveAsync(request.Id.ToString());
            return request.Id;
        }
    }
}