using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SciQuery.Service.Interfaces;
[Authorize]
public class NotificationHub(INotificationService notificationService) : Hub
{
    private readonly INotificationService _notificationService = notificationService;

    // Bildirishnoma yuborish metodini yaratish
    [Authorize]
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
        await _notificationService.GetAllNotificationsByUserId(userId);
    }
    public string GetConnectionId()
    {
        return Context.ConnectionId;
    }
    public async Task MarkAsRead(int notificationId)
    {
        await _notificationService.MarkNotificationAsRead(notificationId);
    }
}

