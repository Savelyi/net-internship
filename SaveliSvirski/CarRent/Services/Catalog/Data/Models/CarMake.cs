namespace Data.Models
{
    public class CarMake : BaseModel
    {
        public string Make { get; set; }
        public virtual ICollection<CarModel> CarModelInfo { get; set; }
    }
}