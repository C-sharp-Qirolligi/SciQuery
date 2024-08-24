using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SciQuery.Domain.Entities;
using SciQuery.Domain.UserModels;
using SciQuery.Infrastructure.Persistance.DbContext;
using System.Text;
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
        await _notificationService.GetAllNotificationsByUserId(userId);
    }
    public string GetConnectionId()
    {
        return Context.ConnectionId;
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
    public async Task GetAllNotificationsByUserId(string userId)
    {
        var notifications = await _context.Notifications.Where(n => n.UserId == userId).ToListAsync();
        
        foreach(var notification in notifications)
        {
            await _hubcontext.Clients.Group(userId).SendAsync("ReceiveNotification", notification);
        }
    }
    public async Task NotifyUser(string userId, string message)
    {
        if (userId == null)
        {
            return;
        }
        Notification notification = new Notification()
        {
            UserId = userId,
            IsRead = false,
            TimeSpan = DateTime.UtcNow,
            Message = message
        };

        await _hubcontext.Clients.Group(userId).SendAsync("ReceiveNotification", notification);
        await AddNotification(notification);
    }

    public async Task AddNotification(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
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

