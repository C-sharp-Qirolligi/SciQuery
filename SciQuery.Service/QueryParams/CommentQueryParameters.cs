namespace SciQuery.Service.QueryParams;
public class CommentQueryParameters: QueryParametersBase
{
    public int? PostId { get; set; }
    public int? PostType { get; set; }
    public string? UserId { get; set; }
}
