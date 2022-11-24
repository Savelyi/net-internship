using BusinessLogic.Dto;

namespace CQRS.Commands
{
    public class CarToUpdateCommand : CarToManipulateCommand
    {
        public Guid Id { get; set; }

        public CarToUpdateCommand(CarToUpdateDto carToUpdate)
        {
            VehicleNumber = carToUpdate.VehicleNumber;
            Model = carToUpdate.Model;
            Make = carToUpdate.Make;
            RentPrice = carToUpdate.RentPrice;
        }
        
        public CarToUpdateCommand()
        {
        }
    }
}