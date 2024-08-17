using SciQuery.Domain.UserModels;

namespace SciQuery.Domain.Entities;

public class Question
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public int Votes { get; set; } = 0;
    public List<string>? ImagePaths { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? UpdatedDate { get; set; } = DateTime.Now;
    public string UserId { get; set; }
    public User User { get; set; }
    public virtual ICollection<Answer> Answers { get; set; }
    public virtual ICollection<Comment> Comments { get; set; }
    public virtual ICollection<QuestionTag> QuestionTags { get; set; }
    public Question()
    {
        ImagePaths = new List<string>();
        Answers = new List<Answer>();
        Comments = new List<Comment>();
        QuestionTags = new List<QuestionTag>();
    }

}

