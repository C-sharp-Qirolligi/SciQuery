using Microsoft.AspNetCore.Identity;
using SciQuery.Domain.Entities;
using System.Globalization;

namespace SciQuery.Domain.UserModels;

public class User : IdentityUser
{
    public int Reputation {  get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastLogindate {  get; set; }
    public virtual ICollection<Answer> Answers{ get; set; }
    public virtual ICollection<Comment> Comments{ get; set; }
    public virtual ICollection<Question> Questions{ get; set; }
    public virtual ICollection<Notification> Notifications { get; set; }
    public User()
    {
        Answers = new List<Answer>();
        Comments = new List<Comment>();
        Questions = new List<Question>();
        Notifications = new List<Notification>();
    }
}
