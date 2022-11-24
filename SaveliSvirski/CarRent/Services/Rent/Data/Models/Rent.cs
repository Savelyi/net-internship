namespace Data.Models
{
    public class Rent : BaseModel
    {
        public Guid UserId { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Closed { get; set; } = null;
        public bool IsClosed { get; set; } = false;
        public Guid CarId { get; set; }
        public virtual Car CarInfo { get; set; }
    }
}