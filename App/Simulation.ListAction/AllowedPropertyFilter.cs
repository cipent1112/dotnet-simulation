namespace Simulation.ListAction;

public class AllowedPropertyFilter
{
    public string  ParamKey       { get; set; } = null!;
    public Type?   RelationClass  { get; set; }
    public string? RelationProperty   { get; set; }
    public Type?   FilterClass    { get; set; }
    public string? FilterProperty { get; set; }
}