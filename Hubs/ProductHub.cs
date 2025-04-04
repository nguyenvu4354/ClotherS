using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ClotherS.Hubs
{
    public class ProductHub : Hub
    {
        public async Task SendUpdate()
        {
            await Clients.All.SendAsync("ReceiveProductUpdate");
        }
    }
}
