using AutoMapper;
using BusinessLogic.Dto;
using CQRS.Queries;
using Data.Contracts;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SharedModels.Cache;
using SharedModels.ErrorModels;

namespace CQRS.Handlers.QueryHandler
{
    public class GetCarByIdHandler : IRequestHandler<GetCarByIdQuery, CarToShowDto>
    {
        private readonly IRepositoryManager repository;
        private readonly IMapper mapper;
        private readonly IDistributedCache cache;

        public GetCarByIdHandler(IRepositoryManager repository, IMapper mapper,
            IDistributedCache cache)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.cache = cache;
        }

        public async Task<CarToShowDto> Handle(GetCarByIdQuery request, CancellationToken cancellationToken)
        {
            CarToShowDto carToShowDto;
            if (!cache.TryGetValue(request.Id.ToString(), out carToShowDto))
            {
                var carModel = await repository.Models.GetByIdAsync(request.Id, cancellationToken, true);
                if (carModel == null)
                {
                    throw new NotFoundException($"Car with Id {request.Id} was not found");
                }

                carToShowDto = mapper.Map<CarToShowDto>(carModel);
                await cache.SetAsync(request.Id.ToString(), carToShowDto);
            }

            return carToShowDto;
        }
    }
}