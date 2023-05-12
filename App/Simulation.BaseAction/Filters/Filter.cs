namespace Simulation.BaseAction.Filters;

public class Filter
{
    public string Property    { get; set; } = null!;
    public string ValueMethod { get; set; } = "QueryParam";
    public string Operand     { get; set; } = "&&";
    public string Value       { get; set; } = null!;
}