namespace SciQuery.Service.DTOs.User;

public class UserForUpdatesDto
{
    public string Id { get; set; } 
    public string UserName { get; set; }
    public string? ImagePath { get; set; }
    public string Email { get; set; }
    public int Reputation { get; set; }
}
