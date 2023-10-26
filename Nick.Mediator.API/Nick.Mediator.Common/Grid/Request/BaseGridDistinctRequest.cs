namespace Nick.Mediator.Common.Grid.Request;

public abstract class BaseGridDistinctRequest
{
    protected BaseGridDistinctRequest()
    {
        Search = new GridSearchRequest();
    }

    public GridSearchRequest Search { get; set; }
    public List<GridFilterRequest> Filters { get; set; }
    public string FieldName { get; set; }
}