using SciQuery.Domain.Entities;
using SciQuery.Service.DTOs.Answer;
using SciQuery.Service.DTOs.Comment;

namespace SciQuery.Service.DTOs.Question;

public class QuestionForUpdateDto
{
    public string Title { get; set; }
    public string Body { get; set; }
}
