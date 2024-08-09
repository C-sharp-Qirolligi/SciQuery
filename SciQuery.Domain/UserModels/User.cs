using Microsoft.AspNetCore.Identity;
using SciQuery.Domain.Entities;
using System.Globalization;

namespace SciQuery.Domain.UserModels;

public class User : IdentityUser
{
    public string? ProfileImagePath { get; set; }
    public int Reputation {  get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastLogindate {  get; set; }
    public virtual ICollection<Answer> Answers{ get; set; }
    public virtual ICollection<Question> Questions{ get; set; }
    public virtual ICollection<Comment> Comments{ get; set; }
    public User()
    {
        Answers = new List<Answer>();
        Questions = new List<Question>();
        Comments = new List<Comment>();
    }
}
