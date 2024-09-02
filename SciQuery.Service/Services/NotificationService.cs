using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SciQuery.Domain.Entities;
using SciQuery.Infrastructure.Persistance.DbContext;
using SciQuery.Service.DTOs.Notification;
using SciQuery.Service.Interfaces;
using SciQuery.Service.Mappings.Extensions;
using SciQuery.Service.Pagination.PaginatedList;
using SciQuery.Service.QueryParams;

namespace SciQuery.Service.Services;

public class NotificationService(IHubContext<NotificationHub> hubcontext,
    SciQueryDbContext context,
    IMapper mapper,
    IFileManagingService fileManaging
    ) : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubcontext = hubcontext;
    private readonly SciQueryDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IFileManagingService _fileManaging = fileManaging;

    public async Task<PaginatedList<NotificationDto>> GetNotificationsByUserId(string userId,
        NotificationQueryParameters queryParameters)
    {
        var query = _context.Notifications
            .AsQueryable()
            .Where(n => n.UserId == userId)
            .AsNoTracking();
        
        if (!string.IsNullOrWhiteSpace(queryParameters.SortBy) && queryParameters.SortBy == QuerySortingParametersConstants.Unread)
        {
            query = query.Where(n => n.IsRead == false);
        }

        if (!string.IsNullOrWhiteSpace(queryParameters.SortBy) && queryParameters.SortBy == QuerySortingParametersConstants.MostNew)
        {
            query = query.OrderByDescending(n => n.TimeSpan);
        }
            
        var notifications = await query.ToPaginatedList<NotificationDto,Notification>
            (_mapper.ConfigurationProvider,queryParameters.PageNumber,queryParameters.PageSize);
        
        foreach(var notification in notifications.Data)
        {
            notification.User.Image = await _fileManaging.DownloadFileAsync(notification.User.ImagePath,"UserImages");
        }

        return notifications;
    }
    public async Task GetUnreadNotificationsByUserId(string userId)
    {
        var notifications = await _context.Notifications.Where(n => n.UserId == userId && n.IsRead == false).ToListAsync();
        foreach (var notification in notifications)
        {
            await _hubcontext.Clients.Group(userId).SendAsync("ReceiveNotification", notification);
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
