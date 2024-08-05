namespace SciQuery.Service.QueryParams;

public class QuestionQueryParameters : QueryParametersBase
{
    public int? AnswerMaxCount { get; set; }
    public int? AnswerMinCount { get; set; }
    public bool? NewAsc { get; set; }
    public DateTime? LastDate { get; set; }
    public ICollection<string>? Tags { get; set; }
}
