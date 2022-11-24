using AutoMapper;
using BusinessLogic.Dto;
using CQRS.Queries;
using Data.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedModels.ErrorModels;

namespace CQRS.Handlers.QueryHandler
{
    public class GetCarsHandler : IRequestHandler<GetCarsQuery, IEnumerable<CarToShowDto>>
    {
        private readonly IRepositoryManager repository;
        private readonly IMapper mapper;

        public GetCarsHandler(IRepositoryManager repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<CarToShowDto>> Handle(GetCarsQuery request, CancellationToken cancellationToken)
        {
            Guid makeId;
            if (Guid.TryParse(request.MakeId, out makeId))
            {
                if (await repository.Makes.GetByIdAsync(makeId, cancellationToken, false) == null)
                {
                    throw new NotFoundException($"Make with Id {request.MakeId} was not found");
                }

                var carModels = await repository.Models.GetByCondition(c => c.CarMakeId == makeId, cancellationToken, true).ToListAsync();
                var carModelsToShow = mapper.Map<IEnumerable<CarToShowDto>>(carModels);
                return carModelsToShow;
            }
            else
            {
                var carModels = await repository.Models.GetAllAsync(request.requestParameters, cancellationToken, true);
                var carModelsDto = mapper.Map<IEnumerable<CarToShowDto>>(carModels);
                return carModelsDto;
            }
        }
    }
}