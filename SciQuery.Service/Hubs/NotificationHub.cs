using Microsoft.AspNetCore.SignalR;
using SciQuery.Service.Services;
using System.Threading.Tasks;

namespace SciQuery.Service.Hubs
{
    //public class NotificationHub : Hub
    //{
    //    public int reulst() {  return 0; }
    //    public async Task SendNotification(string userId, string message)
    //    {
    //        await Clients.User(userId).SendAsync("ReceiveNotification", message);
    //    }

    //}
    public class NotificationHub : Hub
    {
        private readonly IUserConnectionManager _userConnectionManager;

        public NotificationHub(IUserConnectionManager userConnectionManager)
        {
            _userConnectionManager = userConnectionManager;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                _userConnectionManager.AddConnection(userId, Context.ConnectionId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _userConnectionManager.RemoveConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}