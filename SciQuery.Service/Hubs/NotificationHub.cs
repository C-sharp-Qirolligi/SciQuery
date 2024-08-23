using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SciQuery.Domain.Entities;
using SciQuery.Infrastructure.Persistance.DbContext;
[Authorize]
public class NotificationHub(NotificationService notificationService) : Hub
{
    private readonly NotificationService _notificationService = notificationService;

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
    }
    public async Task MarkAsRead(int notificationId)
    {
        await _notificationService.MarkNotificationAsRead(notificationId);
        var notification = await _notificationService.GetNotificationById(notificationId);
        if (notification != null)
        {
            await Clients.User(notification.UserId).SendAsync("NotificationRead", notificationId);
        }
    }
}
public class NotificationService(IHubContext<NotificationHub>hubcontext,SciQueryDbContext context)
{
    private readonly IHubContext<NotificationHub> _hubcontext = hubcontext;
    private readonly SciQueryDbContext _context = context;

    public async Task NotifyUser(string userId, string message)
    {
        await _hubcontext.Clients.Group(userId).SendAsync("ReceiveNotification", message);
    }

    public async Task AddNotification(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Notification>> GetNotificationsForUser(string userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .ToListAsync();
    }

    public async Task MarkNotificationAsRead(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Notification> GetNotificationById(int notificationId)
    {
        return await _context.Notifications.FindAsync(notificationId);
    }
}

