namespace BusinessLogic.Dto
{
    public class RentToShowDto
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Closed { get; set; }
        public bool IsClosed { get; set; }
        public Guid CarId { get; set; }
    }
}