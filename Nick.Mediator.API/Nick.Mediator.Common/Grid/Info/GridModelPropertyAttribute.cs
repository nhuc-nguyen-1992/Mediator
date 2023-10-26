namespace Nick.Mediator.Common.Grid.Info;

[AttributeUsage(AttributeTargets.Property)]
public class GridModelPropertyAttribute : System.Attribute
{
    public string DisplayName { get; set; }
    public string InternalFieldName { get; set; }
    // public bool? AllowFilter { get; set; }
    // public bool? AllowSort { get; set; }
    public bool CannotFilter { get; set; }
    public bool CannotSort { get; set; }
    public bool IsIgnored { get; set; }
    public bool IgnoreFromExport { get; set; }
}