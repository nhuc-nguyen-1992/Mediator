using System.ComponentModel;

namespace Nick.Mediator.Common.Grid.Request;

public class GridSearchRequest
{
    [Description("Search text")]
    public string SearchText { get; set; }
}