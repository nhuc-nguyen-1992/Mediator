using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nick.Mediator.Common.Grid.Request;

public class GridFilterRequest
{
    [Description("Name of a field used to filter with")]
    public string? FieldName { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    [Description("Filter type. E.g: GreaterThan, Equal")]
    public GridFilterType FilterType { get; set; }

    [Description("Filter value")] public string? Value { get; set; }

    [Description("Additional filter value")]
    public string? OtherValue { get; set; }

    [Description("List of filter values")] public List<string?> Values { get; set; }
}