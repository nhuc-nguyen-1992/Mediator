using System.ComponentModel;

namespace Nick.Mediator.Common.Grid.Request;

public class GridSortRequest
{
    [Description("Name of a field used to sort its values")]
    public string FieldName { get; set; }
    [Description("Sort in ascending or descending order")]
    public bool IsAscending { get; set; }
}