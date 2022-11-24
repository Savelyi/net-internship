using BusinessLogic.Dto;
using Data.RequestFeatures;
using MediatR;

namespace CQRS.Queries
{
    public class GetCarsQuery : IRequest<IEnumerable<CarToShowDto>>
    {
        public string MakeId { get; set; }
        public RequestParameters requestParameters { get; set; }

        public GetCarsQuery(RequestParameters requestParameters, string makeId)
        {
            this.requestParameters = requestParameters;
            MakeId = makeId;
        }
    }
}