namespace SciQuery.Service.DTOs.Answer;

public class AnswerForUpdateDto
{
    public string Body { get; set; }
    public int QuestionId { get; set; }
    public string UserId { get; set; }
    public List<string>? ImagePaths { get; set; }
}
