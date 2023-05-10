using System.Linq.Dynamic.Core;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Simulation.ListAction;

public class PagedAction
{
    private List<AllowedPropertyFilter>? AllowedPropertyFilters { get; }

    public PagedAction(List<AllowedPropertyFilter>? allowedPropertyFilters)
    {
        AllowedPropertyFilters = allowedPropertyFilters;
    }

    public IQueryable<T> ApplyFilter<T>(IQueryable<T> query, List<PropertiesFilter> filters)
    {
        var parentClassType = typeof(T);
        var parentProps     = parentClassType.GetProperties();

        if (AllowedPropertyFilters == null) return query;

        var operatorFields = typeof(OperatorsFilter)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(field => field.FieldType == typeof(string) && field.IsLiteral && !field.IsInitOnly)
            .Select(field => field.GetValue(null)?.ToString())
            .ToArray();

        foreach (var filter in filters)
        {
            var alProp = AllowedPropertyFilters.FirstOrDefault(_ => _.ParamKey == filter.Field);
            if (alProp == null) continue;

            if (!operatorFields.Contains(filter.Operator)) continue;

            string? filterProp;
            if (alProp.FilterProperty == null)
                filterProp  = alProp.RelationProperty ?? alProp.ParamKey;
            else filterProp = alProp.FilterProperty;

            var relationClass = alProp.RelationClass ?? parentClassType;
            var relationProp  = alProp.RelationProperty ?? filterProp;
            var filterClass   = alProp.FilterClass ?? relationClass;

            var validRelationProp = relationClass.GetProperty(relationProp);
            var validFilterProp   = filterClass.GetProperty(filterProp);

            if (validRelationProp == null) throw new Exception("Invalid relation property.");
            if (validFilterProp == null) throw new Exception("Invalid filter property");

            filterClass   = validFilterProp.DeclaringType;
            filterProp    = validFilterProp.Name;
            relationClass = validRelationProp.DeclaringType;
            relationProp  = validRelationProp.Name;

            int relationMode;
            if (relationClass == parentClassType)
            {
                if (!parentProps.Any(_ => _.Name.Equals(relationProp)))
                    throw new Exception($"{relationProp} property not exist in {parentClassType.Name}");

                relationMode = relationClass != filterClass ? 1 : 0;
            }
            else
            {
                if (!parentProps.Any(_ => _.Name.Equals(relationClass?.Name)))
                    throw new Exception($"{relationProp} property is not exist in {parentClassType.Name}.");

                var relationPropType = validRelationProp.PropertyType;
                if (!relationPropType.IsGenericType)
                    throw new Exception($"{relationProp} property is not generic type.");
                if (relationPropType.GetGenericTypeDefinition() != typeof(ICollection<>))
                    throw new Exception($"{relationProp} property is not ICollection type.");

                relationMode = 2;
            }

            Console.WriteLine(JsonConvert.SerializeObject(new
            {
                alProp.ParamKey,
                RelationClass    = relationClass?.Name,
                RelationProperty = relationProp,
                FilterClass      = filterClass?.Name,
                FilterProperty   = filterProp,
                Mode             = relationMode
            }, Formatting.Indented));


            query = BuildQuery(
                query: query,
                operate: filter.Operator,
                relationMode: relationMode,
                property: relationMode switch
                {
                    0 => filterProp,
                    1 => $"{relationProp}.{filterProp}",
                    2 => $"{relationClass?.Name}.{relationProp}",
                    _ => throw new Exception("Invalid relation mode.")
                },
                propertyType: validFilterProp.PropertyType,
                value: filter.Value,
                filterProperty: filterProp
            );
        }

        return query;
    }

    private static IQueryable<T> BuildQuery<T>(
        IQueryable<T> query, string operate, int relationMode, string property,
        Type propertyType, object value, string filterProperty
    )
    {
        if (operate.Equals(OperatorsFilter.BetweenOperator))
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

            if (!allowBetween) return query;

            object from;
            object until;

            if (propertyType == typeof(DateTime))
            {
                from  = DateTime.Parse(((JArray)value)[0].ToString());
                until = DateTime.Parse(((JArray)value)[1].ToString());
            }
            else
            {
                from  = Convert.ToDouble(((JArray)value)[0]);
                until = Convert.ToDouble(((JArray)value)[1]);
            }

            query = relationMode switch
            {
                0 or 1 => query.Where($"{property} >= @0 && {property} <= @1", from, until),
                2 => query.Where($"{property}.Any(x => x.{filterProperty} >= @0 && x.{filterProperty} <= @1)", from,
                    until),
                _ => query
            };

            return query;
        }

        query = operate switch
        {
            OperatorsFilter.EqualOperator => relationMode switch
            {
                0 or 1 => query.Where($"{property} == @0", value),
                2      => query.Where($"{property}.Any(x => x.{filterProperty} == @0)", value),
                _      => query
            },
            OperatorsFilter.NotEqualOperator => relationMode switch
            {
                0 or 1 => query.Where($"{property} != @0", value),
                2      => query.Where($"{property}.Any(x => x.{filterProperty} != @0)", value),
                _      => query
            },
            OperatorsFilter.LikeOperator => relationMode switch
            {
                0 or 1 => query.Where($"{property}.Contains(@0)", value),
                2      => query.Where($"{property}.Any(x => x.{filterProperty}.Contains(@0))", value),
                _      => query
            },
            OperatorsFilter.NotLikeOperator => relationMode switch
            {
                0 or 1 => query.Where($"!{property}.Contains(@0)", value),
                2      => query.Where($"{property}.Any(x => !x.{filterProperty}.Contains(@0))", value),
                _      => query
            },
            OperatorsFilter.LessThanOperator => relationMode switch
            {
                0 or 1 => query.Where($"{property} < @0", value),
                2      => query.Where($"{property}.Any(x => x.{filterProperty} < @0)", value),
                _      => query
            },
            OperatorsFilter.LessThanEqualOperator => relationMode switch
            {
                0 or 1 => query.Where($"{property} <= @0", value),
                2      => query.Where($"{property}.Any(x => x.{filterProperty} <= @0)", value),
                _      => query
            },
            OperatorsFilter.GreaterThanOperator => relationMode switch
            {
                0 or 1 => query.Where($"{property} > @0", value),
                2      => query.Where($"{property}.Any(x => x.{filterProperty} > @0)", value),
                _      => query
            },
            OperatorsFilter.GreaterThanEqualOperator => relationMode switch
            {
                0 or 1 => query.Where($"{property} >= @0", value),
                2      => query.Where($"{property}.Any(x => x.{filterProperty} >= @0)", value),
                _      => query
            },
            OperatorsFilter.InOperator => relationMode switch
            {
                0 or 1 => query.Where($"{(JArray)value}.Contains(@0)", property),
                2      => query.Where($"{property}.Any(x => {(JArray)value}.Contains(@0))", $"x.{filterProperty}"),
                _      => query
            },
            OperatorsFilter.NotInOperator => relationMode switch
            {
                0 or 1 => query.Where($"!{(JArray)value}.Contains(@0)", property),
                2      => query.Where($"{property}.Any(x => !{(JArray)value}.Contains(@0))", $"x.{filterProperty}"),
                _      => query
            },
            _ => query
        };

        return query;
    }
}