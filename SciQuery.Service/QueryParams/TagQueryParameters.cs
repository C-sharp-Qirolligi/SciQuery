namespace SciQuery.Service.QueryParams;
public class TagQueryParameters : QueryParametersBase
{
    public bool? SortDescending { get; set; }
    public  bool? Popular { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

}
