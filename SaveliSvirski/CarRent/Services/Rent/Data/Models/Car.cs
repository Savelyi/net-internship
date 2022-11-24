namespace Data.Models
{
    public class Car : BaseModel
    {
        public bool IsAvailable { get; set; } = true;
        public virtual ICollection<Rent> RentInfo { get; set; }
    }
}