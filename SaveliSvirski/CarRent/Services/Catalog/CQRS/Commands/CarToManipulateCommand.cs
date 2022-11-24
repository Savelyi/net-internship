using MediatR;

namespace CQRS.Commands
{
    public abstract class CarToManipulateCommand : IRequest<Guid>
    {
        public string VehicleNumber { get; set; }
        public string Model { get; set; }
        public string Make { get; set; }
        public decimal RentPrice { get; set; }
    }
}