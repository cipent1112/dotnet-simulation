namespace Simulation.BaseAction.Constants;

public static class Operator
{
    private const string And = "&&";
    private const string Or  = "||";

    private static Dictionary<string, string> List => new()
    {
        { And, "&&" },
        { Or, "||" }
    };

    public static string Get(string operate) => List[operate];
}