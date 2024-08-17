using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
[Authorize]
public class NotificationHub : Hub
{
    // Bildirishnoma yuborish metodini yaratish
    public async Task SendNotification(string message)
    {
        // Foydalanuvchiga bildirishnoma yuborish
        await Clients.All.SendAsync("ReceiveNotification", message);
    }
    [Authorize]
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        await base.OnConnectedAsync();
    }

}
public class NotificationService(IHubContext<NotificationHub>hubcontext)
{
    private readonly IHubContext<NotificationHub> _hubcontext = hubcontext;

    public async Task NotifyUser(string userId, string message)
    {
        await _hubcontext.Clients.Group(userId).SendAsync("ReceiveNotification", message);

    }
}

