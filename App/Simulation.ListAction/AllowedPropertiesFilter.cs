namespace Simulation.ListAction;

public class AllowedPropertiesFilter
{
    public string    Key                { get; set; } = null!;
    public string[]? RelationProperties { get; set; }
    public string?   FilterProperty     { get; set; }
}