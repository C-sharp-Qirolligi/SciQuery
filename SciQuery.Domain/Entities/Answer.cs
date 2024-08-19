﻿using SciQuery.Domain.UserModels;

namespace SciQuery.Domain.Entities;
public class Answer
{
    public int Id { get; set; }
    public string? Body { get; set; }
    public int Votes { get; set; } = 0;
    public List<string>? ImagePaths { get; set; }
    public List<string>? VotedUserIds { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? UpdatedDate { get; set; } = DateTime.Now;
    public int QuestionId { get; set; }
    public Question Question { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }

    public virtual ICollection<Comment> Comments{ get; set; }
    public Answer()
    {
        ImagePaths = new List<string>();
        VotedUserIds = new List<string>();
    }

}
