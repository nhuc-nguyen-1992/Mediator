namespace Nick.Mediator.Common.Grid.Info;

public class GridModelPropertyInfo
{
    public string? DataType { get; set; }
    public string DisplayName { get; set; }
    public string InternalFieldName { get; set; }
    public string FieldName { get; set; }
    public string FieldNameCamelCase { get; set; }
    public bool AllowFilter { get; set; }
    public bool AllowSort { get; set; }
    public bool IgnoreFromExport { get; set; }

    //if you want to add something like "allow edit" or similar, write them down here
}