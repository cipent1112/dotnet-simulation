using System.Collections;
using Newtonsoft.Json.Linq;

namespace Simulation.BaseAction.Filters;

public class Filter
{
    public string  Property      { get; set; } = null!;
    public string  Conjunction   { get; set; } = "&&";
    public string  FilterOperand { get; set; } = "=";
    public string? FilterValue   { get; set; }
    public string? FilterKey     { get; set; }

    public object? GetFilterValue(List<QueryParam> queryParams)
    {
        var requestedValue = queryParams.FirstOrDefault(_ => _.Field == FilterKey)?.Value;

        if (requestedValue is JArray)
        {
            Console.WriteLine("Is JArray");
            var jArray = (JArray)requestedValue;
            object[] objectArray = jArray.ToObject<object[]>();

            requestedValue = objectArray;
        }
        
        if (FilterKey != null) return requestedValue;
        return FilterValue ?? null;
    }
    
    public string GetFilterOperand(List<QueryParam> queryParams)
    {
        var queryOperator = queryParams.FirstOrDefault(_ => _.Field == FilterKey);
        return queryOperator != null ? queryOperator.Operator : FilterOperand;
    }
}