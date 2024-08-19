using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciQuery.Service.QueryParams;

public abstract class QueryParametersBase
{
    public int PageSize { get; set; } = 15;
    public int PageNumber { get; set; } = 1;
    public string? Search { get; set; }
}
