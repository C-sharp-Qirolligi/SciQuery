namespace SciQuery.Service.QueryParams;

public class QuestionQueryParameters : QueryParametersBase
{
    public bool? NoAnswers { get; set; }
    public bool? NoAcceptedAnswer { get; set; }
    public bool? MostNew { get; set; }
    public bool? MostRecently { get; set; }
    public bool? MostVoted { get; set; }
    public ICollection<string>? Tags { get; set; }

}
