using Data.Contracts;
using Data.RequestFeatures;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SharedModels.CatalogEvents;
using SignalR;

namespace CatalogApi.Consumers
{
    public class GetInitAmountConsumer : IConsumer<IGetInitAmountEvent>
    {
        private readonly IHubContext<CatalogHub, ICatalogHub> catalogHub;
        private readonly IRepositoryManager repository;

        public GetInitAmountConsumer(IHubContext<CatalogHub, ICatalogHub> catalogHub, IRepositoryManager repository)
        {
            this.catalogHub = catalogHub;
            this.repository = repository;
        }
        public async Task Consume(ConsumeContext<IGetInitAmountEvent> context)
        {
            var count=await repository.Models.GetAll().CountAsync(e=>e.IsAvailable);
            await catalogHub.Clients.Client(context.Message.ClientId).ReceiveCount(count);
        }
    }
}
