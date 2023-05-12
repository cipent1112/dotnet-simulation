using System.Linq.Dynamic.Core;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Simulation.ListAction;

public class ListAction
{
    private List<AllowedPropertiesFilter>? AllowedFilterProperties { get; }

    public ListAction(List<AllowedPropertiesFilter>? allowedFilterProperties)
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
        }

        return query;
    }

    private static string BuildCondition(IReadOnlyList<int> relationTypes, string property, string operand, string[]? relations)
    {
        string propertyCondition;

        if (new List<string>
            {
                OperatorsFilter.EqualOperator, OperatorsFilter.NotEqualOperator,
                OperatorsFilter.LessThanOperator, OperatorsFilter.LessThanEqualOperator,
                OperatorsFilter.GreaterThanOperator, OperatorsFilter.GreaterThanEqualOperator
            }.Contains(operand))
        {
            propertyCondition = $"{property} {operand} @0";
        }
        else if (new List<string> { OperatorsFilter.LikeOperator }.Contains(operand))
        {
            propertyCondition = $"{property}.Contains(@0)";
        }
        else if (new List<string> { OperatorsFilter.NotLikeOperator }.Contains(operand))
        {
            propertyCondition = $"!{property}.Contains(@0)";
        }
        else if (operand.Equals(OperatorsFilter.BetweenOperator))
        {
            propertyCondition = $"{property} >= @0 && {property} <= @1";
        }
        else throw new Exception("Operand not supported");


        if (!(relations?.Length >= 1)) return propertyCondition;

        for (var iC = relationTypes.Count - 1; iC >= 0; iC--)
        {
            propertyCondition = relationTypes[iC] == 2
                ? $"{relations[iC]}.Any({propertyCondition})"
                : $"{relations[iC]}.{propertyCondition}";
        }

        Console.WriteLine(propertyCondition);
        return propertyCondition;
    }

    private static int GetMode(Type propertyType)
    {
        return propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
            ? 2
            : 1;
    }
}