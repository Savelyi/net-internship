namespace Data.Models
{
    public class CarModel : BaseModel
    {
        public string VehicleNumber { get; set; }
        public string Model { get; set; }
        public decimal RentPrice { get; set; }
        public bool IsAvailable { get; set; } = true;
        public Guid CarMakeId { get; set; }
        public virtual CarMake CarMakeInfo { get; set; }
    }
}