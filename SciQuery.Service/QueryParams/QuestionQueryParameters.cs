namespace SciQuery.Service.QueryParams;

public class QuestionQueryParameters : QueryParametersBase
{
    public bool? NoAnswers { get; set; }
    public bool? NoAcceptedAnswer { get; set; }
    public string? SortBy { get; set; } = QuerySortingParametersConstants.MostNew;
    public ICollection<string>? Tags { get; set; }

}
