using SciQuery.Domain.Entities;
using SciQuery.Service.DTOs.User;

namespace SciQuery.Service.DTOs.Comment;

public class CommentDto
{
    public int Id { get; set; }
    public string Body { get; set; }
    public int PostId { get; set; }
    public PostType Post { get; set; }
    public string UserId { get; set; }
    public UserDto User { get; set; }
}
