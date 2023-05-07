namespace Simulation.ListAction;

public class AllowedPropertyFilter
{
    /* `ParamKey` -> the key for custom field name filter */
    public string ParamKey { get; set; } = null!;

    /*
     * `RelationName` is property name of the parent class and
     * When `RelationName` is null than the `ParamKey` as parent property name.
     */
    public string? RelationName { get; set; }

    /*
     * `PropertyName` is property name of the child class and
     * `PropertyName` is a reference for filter nested child property when the `PropertyName` is not null.
     */
    public string? PropertyName { get; set; }
}