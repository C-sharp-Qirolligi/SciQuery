using SciQuery.Domain.Entities;
using SciQuery.Service.DTOs.Notification;
using SciQuery.Service.Pagination.PaginatedList;
using SciQuery.Service.QueryParams;

namespace SciQuery.Service.Interfaces
{
    public interface INotificationService
    {
        Task AddNotification(Notification notification);
        Task GetUnreadNotificationsByUserId(string userId);
        Task<PaginatedList<NotificationDto>> GetNotificationsByUserId(string userId,NotificationQueryParameters queryParameters);
        Task<Notification> GetNotificationById(int notificationId);
        Task MarkNotificationAsRead(int notificationId);
        Task NotifyUser(Notification notification);
    }
}