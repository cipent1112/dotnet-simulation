namespace Simulation.ListAction;

public class AllowedPropertyFilter
{
    /* `ParamKey` -> the key for custom field name filter */
    public string ParamKey { get; set; } = null!;

    /*
     * `PropertyName` is property name of the parent class and
     * When `PropertyName` is null than the `ParamKey` as parent property name.
     */
    public string? PropertyName { get; set; }

    /*
     * `PropertyChildName` is property name of the child class and
     * `PropertyChildName` is a reference for filter nested child property when the `PropertyChildName` is not null.
     */
    public string? ChildPropertyName { get; set; }
}