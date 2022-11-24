using AutoMapper;
using CQRS.Commands;
using Data.Contracts;
using Data.Models;
using Data.RequestFeatures;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Handlers.CommandHandlres
{
    public abstract class CarToManipulateBaseHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : CarToManipulateCommand, IRequest<TResponse>
    {
        protected readonly IMapper mapper;
        protected readonly IRepositoryManager repository;
        protected readonly IValidator<TRequest> validator;

        public CarToManipulateBaseHandler(IMapper mapper, IRepositoryManager repository, IValidator<TRequest> validator)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.validator = validator;
        }

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);

        protected async Task<CarModel> CreateMakeIfNotExistAsync(TRequest request, CancellationToken cancellationToken)
        {
            var carModel = mapper.Map<CarModel>(request);
            var result = await repository.Makes.GetByConditionAsync(e => e.Make.ToLower() == request.Make.ToLower(),
                cancellationToken);

            if (result == null)
            {
                var carMake = new CarMake
                {
                    Id = Guid.NewGuid(),
                    Make = request.Make
                };
                carModel.CarMakeId = carMake.Id;
                await repository.Makes.CreateAsync(carMake, cancellationToken);
            }
            else
            {
                carModel.CarMakeId = result.Id;
            }

            return carModel;
        }
    }
}