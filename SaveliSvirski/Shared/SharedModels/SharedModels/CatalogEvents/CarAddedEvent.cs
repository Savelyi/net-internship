namespace SharedModels.CatalogEvents
{
    public class CarAddedEvent : ICarAddedEvent
    {
        public Guid Id { get; set; }
    }
}