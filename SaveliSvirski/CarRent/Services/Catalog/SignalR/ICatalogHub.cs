using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR
{
    public interface ICatalogHub
    {
        Task CarToAdd();
        Task CarToRemove();
        Task ReceiveCount(int count);
    }
}
