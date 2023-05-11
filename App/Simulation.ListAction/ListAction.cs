using System.Linq.Dynamic.Core;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Simulation.ListAction;

public class ListAction
{
    private List<AllowedFilterProperty>? AllowedFilterProperties { get; }

    public ListAction(List<AllowedFilterProperty>? allowedFilterProperties)
    {
        AllowedFilterProperties = allowedFilterProperties;
    }

    public IQueryable<T> ApplyFilter<T>(IQueryable<T> query, List<PropertiesFilter> filters)
    {
        var parentClass = typeof(T);

        if (AllowedFilterProperties == null) return query;

        var operatorFields = typeof(OperatorsFilter)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(field => field.FieldType == typeof(string) && field.IsLiteral && !field.IsInitOnly)
            .Select(field => field.GetValue(null)?.ToString())
            .ToArray();

        foreach (var filter in filters)
        {
            var alProp = AllowedFilterProperties.FirstOrDefault(_ => _.Key == filter.Field);
            if (alProp == null) continue;
            if (!operatorFields.Contains(filter.Operator)) continue;

            var key                = alProp.Key;
            var value              = filter.Value;
            var relationProperties = alProp.RelationProperties;
            var filterProperty     = alProp.FilterProperty ?? key;

            string condition;

            var propertyInfo = parentClass.GetProperty(filterProperty);
            var propertyType = propertyInfo?.PropertyType;

            if (relationProperties != null)
            {
                var modes         = new int[relationProperties.Length];
                var previousClass = parentClass;

                for (var i = 0; i < relationProperties.Length; i++)
                {
                    propertyInfo = previousClass.GetProperty(relationProperties[i]);
                    if (propertyInfo == null) continue;

                    modes[i] = GetMode(propertyInfo.PropertyType);
                    previousClass = modes[i] == 2
                        ? propertyInfo.PropertyType.GetGenericArguments()[0]
                        : propertyInfo.PropertyType;
                }

                propertyType = previousClass.GetProperty(filterProperty)?.PropertyType;
                condition    = BuildCondition(modes, filterProperty, filter.Operator, relationProperties);
            }
            else condition = BuildCondition(Array.Empty<int>(), filterProperty, filter.Operator, null);

            if (filter.Operator.Equals(OperatorsFilter.BetweenOperator))
            {
                var allowBetween = new[]
                {
                    typeof(DateTime),
                    typeof(double),
                    typeof(int),
                    typeof(long),
                    typeof(float),
                    typeof(decimal)
                }.Contains(propertyType);
                if (!allowBetween)
                    throw new Exception(
                        "Invalid property type, only datetime and number type when using between operator");

                object from;
                object until;

                switch (propertyType == typeof(DateTime))
                {
                    case true:
                        from  = DateTime.Parse(((JArray)value)[0].ToString());
                        until = DateTime.Parse(((JArray)value)[1].ToString());
                        break;
                    default:
                        from  = Convert.ToDouble(((JArray)value)[0]);
                        until = Convert.ToDouble(((JArray)value)[1]);
                        break;
                }

                query = query.Where(condition, from, until);
            }
            else query = query.Where(condition, value);

            Console.WriteLine(condition);
        }

        return query;
    }

    // private static string BuildPropertyCondition(IReadOnlyList<int> modes, string[] relations)
    // {
    // }

    private static string BuildCondition(IReadOnlyList<int> modes, string property, string operand, string[]? relations)
    {
        var latestMode = modes.Count > 0 ? modes[^1] : 1;
        Console.WriteLine($"{string.Join(" ", modes)}");
        var condition = string.Empty;

        if (new List<string>
            {
                OperatorsFilter.EqualOperator, OperatorsFilter.NotEqualOperator,
                OperatorsFilter.LessThanOperator, OperatorsFilter.LessThanEqualOperator,
                OperatorsFilter.GreaterThanOperator, OperatorsFilter.GreaterThanEqualOperator
            }.Contains(operand))
        {
            switch (latestMode)
            {
                case 0 or 1:
                    condition = (relations == null) switch
                    {
                        true => $"{property} {operand} @0",
                        _    => $"{string.Join(".", relations!)}.{property} {operand} @0"
                    };
                    break;
                case 2:
                    var latestCondition = $"{property} {operand} @0";
                    for (var i = relations!.Length - 1; i >= 0; i--)
                    {
                        condition = modes[i] switch
                        {
                            2      => $"{relations[i]}.Any({latestCondition})",
                            0 or 1 => $"{relations[i]}.{condition}",
                            _      => condition
                        };

                        latestCondition = condition;
                    }

                    break;
            }
        }
        else if (new List<string> { OperatorsFilter.LikeOperator, OperatorsFilter.InOperator }.Contains(operand))
        {
            switch (latestMode)
            {
                case 0 or 1:
                    Console.WriteLine($"{string.Join(".", relations!)}");
                    var latestLatestMode = modes.Count > 0 ? modes[^2] : latestMode;
                    if (latestLatestMode == 2)
                    {
                        
                    }

                    condition = operand switch
                    {
                        OperatorsFilter.LikeOperator => (relations == null) switch
                        {
                            true => $"{property}.Contains(@0)",
                            _    => $"{string.Join(".", relations!)}.{property}.Contains(@0)"
                        },
                        OperatorsFilter.InOperator => (relations == null) switch
                        {
                            true => $"@0.Contains({property})",
                            _    => $"@0.Contains({string.Join(".", relations!)}.{property})"
                        },
                        _ => condition
                    };
                    break;
                case 2:
                    var latestCondition = operand switch
                    {
                        OperatorsFilter.LikeOperator => $"{property}.Contains(@0)",
                        OperatorsFilter.InOperator   => $"@0.Contains({property})",
                        _                            => condition
                    };

                    for (var i = relations!.Length - 1; i >= 0; i--)
                    {
                        condition = modes[i] switch
                        {
                            2      => $"{relations[i]}.Any({latestCondition})",
                            0 or 1 => $"{relations[i]}.{condition}",
                            _      => condition
                        };

                        latestCondition = condition;
                    }

                    break;
            }
        }
        else if (new List<string> { OperatorsFilter.NotLikeOperator, OperatorsFilter.NotInOperator }.Contains(operand))
        {
            switch (latestMode)
            {
                case 0 or 1:
                    condition = operand switch
                    {
                        OperatorsFilter.NotLikeOperator => (relations == null) switch
                        {
                            true => $"!{property}.Contains(@0)",
                            _    => $"!{string.Join(".", relations!)}.{property}.Contains(@0)"
                        },
                        OperatorsFilter.NotInOperator => (relations == null) switch
                        {
                            true => $"!@0.Contains({property})",
                            _    => $"!@0.Contains({string.Join(".", relations!)}.{property})"
                        },
                        _ => condition
                    };
                    break;
                case 2:
                    var latestCondition = operand switch
                    {
                        OperatorsFilter.NotLikeOperator => $"!{property}.Contains(@0)",
                        OperatorsFilter.NotInOperator   => $"!@0.Contains({property})",
                        _                               => condition
                    };

                    for (var i = relations!.Length - 1; i >= 0; i--)
                    {
                        condition = modes[i] switch
                        {
                            2      => $"{relations[i]}.Any({latestCondition})",
                            0 or 1 => $"{relations[i]}.{condition}",
                            _      => condition
                        };

                        latestCondition = condition;
                    }

                    break;
            }
        }
        else if (operand.Equals(OperatorsFilter.BetweenOperator))
        {
            switch (latestMode)
            {
                case 0 or 1:
                    condition = (relations == null) switch
                    {
                        true => $"{property} >= @0 && {property} <= @1",
                        _ => $"{string.Join(".", relations!)}.{property} >= @0 " +
                             $"&& {string.Join(".", relations!)}.{property} <= @1"
                    };
                    break;
                case 2:
                    var latestCondition = $"{property} >= @0 && {property} <= @1";
                    for (var i = relations!.Length - 1; i >= 0; i--)
                    {
                        condition = modes[i] switch
                        {
                            2      => $"{relations[i]}.Any({latestCondition})",
                            0 or 1 => $"{relations[i]}.{condition}",
                            _      => condition
                        };

                        latestCondition = condition;
                    }

                    break;
            }
        }


        return condition;
    }

    private static int GetMode(Type propertyType)
    {
        return propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
            ? 2
            : 1;
    }
}