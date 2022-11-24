namespace SharedModels.CatalogEvents
{
    public class CarDeletedEvent : ICarDeletedEvent
    {
        public Guid Id { get; set; }
    }
}