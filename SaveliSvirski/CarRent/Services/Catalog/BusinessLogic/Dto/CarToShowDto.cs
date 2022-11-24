namespace BusinessLogic.Dto
{
    public class CarToShowDto
    {
        public Guid Id { get; set; }
        public string VehicleNumber { get; set; }
        public string Make { get; set; }
        public Guid MakeId { get; set; }
        public string Model { get; set; }
        public decimal RentPrice { get; set; }
        public bool IsAvailable { get; set; }
    }
}