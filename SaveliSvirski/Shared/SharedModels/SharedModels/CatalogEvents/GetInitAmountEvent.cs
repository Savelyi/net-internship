namespace SharedModels.CatalogEvents
{
    public class GetInitAmountEvent : IGetInitAmountEvent
    {
        public string ClientId { get; set; }
    }
}