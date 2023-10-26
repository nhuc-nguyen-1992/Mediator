using System.ComponentModel;

namespace Nick.Mediator.Common.Grid.Request;

public abstract class BaseGridRequest
{
    protected BaseGridRequest()
    {
        Search = new GridSearchRequest();
    }

    [Description("Page number")] public int PageIndex { get; set; }

    [Description("How many items a page can display")]
    public int PageSize { get; set; }

    public GridSearchRequest Search { get; set; }
    public List<GridSortRequest> Sort { get; set; }
    public List<GridFilterRequest> Filters { get; set; }
    public List<GridColumnRequest> Columns { get; set; }
}