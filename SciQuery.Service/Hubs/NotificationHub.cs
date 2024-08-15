using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
    // Bildirishnoma yuborish metodini yaratish
    public async Task SendNotification(string message)
    {
        // Foydalanuvchiga bildirishnoma yuborish
        await Clients.All.SendAsync("ReceiveNotification", message);
    }
}
