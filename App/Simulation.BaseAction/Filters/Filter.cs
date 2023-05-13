namespace Simulation.BaseAction.Filters;

public class Filter
{
    public string  Property      { get; set; } = null!;
    public string  Conjunction   { get; set; } = "&&";
    public string  FilterOperand { get; set; } = "=";
    public string? FilterValue   { get; set; }
    public string? FilterKey     { get; set; }

    public string? GetFilterValue(List<QueryParam> queryParams)
    {
        if (FilterKey != null) return queryParams.FirstOrDefault(_ => _.Field == this.FilterKey)?.Value?.ToString();
        return FilterValue ?? null;
    }
}