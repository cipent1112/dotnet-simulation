using Newtonsoft.Json.Linq;
using Simulation.BaseAction.Constants;

namespace Simulation.BaseAction.Filters;

public class Filter
{
    public string  Property      { get; set; } = null!;
    public string  Conjunction   { get; set; } = "&&";
    public string  FilterOperand { get; set; } = "=";
    public string? FilterValue   { get; set; }
    public string? FilterKey     { get; set; }

    public object? GetFilterValue(QueryParam filterQueryParam)
    {
        var requestedValue                                  = filterQueryParam.Value;
        if (requestedValue is JArray jArray) requestedValue = jArray.ToObject<object[]>()!;

        if (FilterKey != null) return requestedValue;
        return FilterValue ?? null;
    }

    public static string GetFilterOperand(QueryParam filterQueryParam)
    {
        if (!Operand.List.Contains(filterQueryParam.Operator))
            throw new Exception($"Invalid filter operand {filterQueryParam.Operator}");
        return filterQueryParam.Operator;
    }
}