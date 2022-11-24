using MediatR;

namespace CQRS.Commands
{
    public class CarToDeleteCommand : IRequest
    {
        public Guid Id { get; set; }

        public CarToDeleteCommand(Guid id)
        {
            Id = id;
        }
    }
}