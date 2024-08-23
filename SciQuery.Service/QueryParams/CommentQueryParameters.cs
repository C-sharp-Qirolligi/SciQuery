namespace SciQuery.Service.QueryParams;
public class CommentQueryParameters: QueryParametersBase
{
    public int? AnswerId { get; set; }
    public int? QuestionId { get; set; }
    public string? UserId { get; set; }
}
