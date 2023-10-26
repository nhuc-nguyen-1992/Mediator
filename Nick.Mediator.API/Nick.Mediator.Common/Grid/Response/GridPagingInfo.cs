using System.ComponentModel;

namespace Nick.Mediator.Common.Grid.Response;
public class GridPagingInfo
{
    [Description("Total of returned records")]
    public int TotalRecord { get; set; }
    [Description("Current page number")]
    public int CurrentPageIndex { get; set; }
    [Description("Current page size")]
    public int CurrentPageSize { get; set; }
}