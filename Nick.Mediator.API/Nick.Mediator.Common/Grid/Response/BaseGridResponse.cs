namespace Nick.Mediator.Common.Grid.Response;

public abstract class BaseGridResponse<T>
{
    public GridPagingInfo Paging { get; set; }
    public List<T> GridData { get; set; }
}