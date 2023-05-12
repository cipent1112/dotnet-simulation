namespace Simulation.BaseAction.Filters;

public class Filter
{
    public string  Property      { get; set; } = null!;
    public string  Conjunction   { get; set; } = "&&";
    public string  FilterOperand { get; set; } = "=";
    public string? FilterValue   { get; set; }
    public string? FilterKey     { get; set; }
}