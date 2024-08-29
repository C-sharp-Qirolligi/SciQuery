namespace SciQuery.Service.QueryParams;

public class NotificationQueryParameters : QueryParametersBase
{
    public string? SortBy { get; set; } = QuerySortingParametersConstants.MostNew;
}
