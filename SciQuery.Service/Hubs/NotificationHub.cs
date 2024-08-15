using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SciQuery.Service.Services;
using System.Threading.Tasks;

namespace SciQuery.Service.Hubs
{
    public interface INotificationClient
    {
        Task ReceiveNotification(string message);
    }

    [Authorize]
    public class NotificationHub : Hub<INotificationClient>
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.ReceiveNotification($"UserConnected {Context.User.Identity?.Name}");
            await base.OnConnectedAsync();
        }
        
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}