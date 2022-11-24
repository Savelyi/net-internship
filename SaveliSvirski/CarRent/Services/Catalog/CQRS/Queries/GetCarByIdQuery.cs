using BusinessLogic.Dto;
using MediatR;

namespace CQRS.Queries
{
    public class GetCarByIdQuery : IRequest<CarToShowDto>
    {
        public GetCarByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

        public Guid Id { get; set; }
    }
}