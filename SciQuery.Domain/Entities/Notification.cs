using SciQuery.Domain.UserModels;

namespace SciQuery.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public int? QuestionId { get; set; }
    public DateTime TimeSpan { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
}
