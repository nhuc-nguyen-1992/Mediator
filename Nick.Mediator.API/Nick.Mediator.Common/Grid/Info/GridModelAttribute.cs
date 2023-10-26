namespace Nick.Mediator.Common.Grid.Info;

[AttributeUsage(AttributeTargets.Class)]
public class GridModelAttribute : System.Attribute
{
    public string GridName { get; set; }

    public GridModelAttribute(string gridName)
    {
        GridName = gridName;
    }
}