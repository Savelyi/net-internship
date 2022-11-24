namespace SharedModels.CatalogEvents
{
    public interface ICarDeletedEvent
    {
        Guid Id { get; set; }
    }
}