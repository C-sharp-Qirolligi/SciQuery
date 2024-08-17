namespace SciQuery.Service.DTOs.User;

public class UserDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public int Reputation { get; set; }
    public ImageFile Image { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastLogindate { get; set; } = DateTime.Now;
}
