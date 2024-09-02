namespace SciQuery.Service.QueryParams
{
    public class AnswerQueryParameters : QueryParametersBase
    {
        public string? UserId { get; set; }
        public int? QuestionId { get; set; }
    }
}
