using SciQuery.Domain.UserModels;

namespace SciQuery.Domain.Entities;

public class Comment
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public int PostId { get; set; }
    public PostType Post{ get; set; }
    public string Body { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}
