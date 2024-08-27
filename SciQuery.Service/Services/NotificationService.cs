using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SciQuery.Domain.Entities;
using SciQuery.Infrastructure.Persistance.DbContext;
using SciQuery.Service.Interfaces;

namespace SciQuery.Service.Services;

public class NotificationService(IHubContext<NotificationHub> hubcontext, SciQueryDbContext context) : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubcontext = hubcontext;
    private readonly SciQueryDbContext _context = context;
    public async Task GetAllNotificationsByUserId(string userId)
    {
        try
        {
            var notifications = await _context.Notifications.Where(n => n.UserId == userId).ToListAsync();
            foreach (var notification in notifications)
            {
                await _hubcontext.Clients.Group(userId).SendAsync("ReceiveNotification", notification);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task NotifyUser(Notification notification)
    {
        if (notification.UserId == null)
        {
            return;
        }

        await _hubcontext.Clients.Group(notification.UserId).SendAsync("ReceiveNotification", notification);
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
