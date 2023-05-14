namespace Simulation.BaseAction.Filters;

public class AllowedFilter
{
    public List<string>? Relations { get; set; }
    public List<Filter>  Filters   { get; set; } = null!;
}