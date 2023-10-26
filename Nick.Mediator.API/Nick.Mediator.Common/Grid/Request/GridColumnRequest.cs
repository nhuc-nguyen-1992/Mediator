using System.ComponentModel;

namespace Nick.Mediator.Common.Grid.Request;

public class GridColumnRequest
{
    [Description("Name of a field that would be displayed in the grid. E.g: ProductName")]
    public string FieldName { get; set; }
    [Description("A text that would be displayed in the grid as a column name. E.g: Product Name")]
    public string DisplayName { get; set; }
}