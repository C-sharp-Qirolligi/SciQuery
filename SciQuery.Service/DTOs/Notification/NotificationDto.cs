using SciQuery.Service.DTOs.User;

namespace SciQuery.Service.DTOs.Notification;

public class NotificationDto
{
    public int Id { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public int? QuestionId { get; set; }
    public DateTime TimeSpan { get; set; }
    public string UserId { get; set; }
    public UserDto User { get; set; }
}
