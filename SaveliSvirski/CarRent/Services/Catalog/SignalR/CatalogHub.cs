using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SharedModels.CatalogEvents;

namespace SignalR
{
    [Authorize]
    public class CatalogHub : Hub<ICatalogHub>
    {
        private readonly IBus bus;

        public CatalogHub(IBus bus)
        {
            this.bus = bus;
        }

        public async Task GetInitCount()
        {
            await bus.Publish<IGetInitAmountEvent>(new GetInitAmountEvent
            {
                ClientId = Context.ConnectionId
            });
        }
    }
}