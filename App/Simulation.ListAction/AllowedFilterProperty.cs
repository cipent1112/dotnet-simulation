namespace Simulation.ListAction;

public class AllowedFilterProperty
{
    public string    Key                { get; set; } = null!;
    public string[]? RelationProperties { get; set; }
    public string?   FilterProperty     { get; set; }
}