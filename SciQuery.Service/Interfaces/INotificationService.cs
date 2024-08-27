using SciQuery.Domain.Entities;

namespace SciQuery.Service.Interfaces
{
    public interface INotificationService
    {
        Task AddNotification(Notification notification);
        Task GetAllNotificationsByUserId(string userId);
        Task<Notification> GetNotificationById(int notificationId);
        Task MarkNotificationAsRead(int notificationId);
        Task NotifyUser(Notification notification);
    }
}