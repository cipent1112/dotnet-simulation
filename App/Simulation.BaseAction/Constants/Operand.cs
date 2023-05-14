using System.Reflection;

namespace Simulation.BaseAction.Constants;

public static class Operand
{
    public const string EqualOperator         = "=";
    public const string NotEqualOperator      = "!=";
    public const string LikeOperator          = "LIKE";
    public const string NotLikeOperator       = "NOT LIKE";
    public const string BetweenOperator       = "BETWEEN";
    public const string LessThanOperator      = "<";
    public const string LessThanEqualOperator = "<=";
    public const string GreaterThanOperator   = ">";

    public const string GreaterThanEqualOperator = ">=";
    // public const string InOperator               = "IN";
    // public const string NotInOperator            = "NOT IN";


    public static IEnumerable<string?> List => typeof(Operand)
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(field => field.FieldType == typeof(string) && field.IsLiteral && !field.IsInitOnly)
        .Select(field => field.GetValue(null)?.ToString())
        .ToArray();
}