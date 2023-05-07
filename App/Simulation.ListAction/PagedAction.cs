using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Simulation.ListAction;

public class PagedAction
{
    private List<AllowedPropertyFilter>? AllowedPropertyFilters { get; set; }

    public PagedAction(List<AllowedPropertyFilter>? allowedPropertyFilters)
    {
        AllowedPropertyFilters = allowedPropertyFilters;
    }

    public IQueryable<T> ApplyFilters<T>(IQueryable<T> query, List<PropertiesFilter> filterList)
    {
        var filteredQuery    = query;
        var parentProperties = typeof(T).GetProperties();

        Console.WriteLine();
        Console.WriteLine($"{typeof(T).Name} properties:");
        parentProperties.ToList().ForEach(_ => Console.WriteLine($"-> {_.Name}"));

        Console.WriteLine();
        Console.WriteLine("Allowed Property Filters: ");
        AllowedPropertyFilters!.ForEach(_ =>
        {
            Console.WriteLine($"\nParam Key\t: {_.ParamKey}");
            Console.WriteLine($"Relation Name\t: {_.RelationName}");
            Console.WriteLine($"Property Name\t: {_.PropertyName}");
        });
        Console.WriteLine();

        if (AllowedPropertyFilters == null) return filteredQuery;

        var parentParameter = Expression.Parameter(typeof(T), BuildLambdaParameterName(typeof(T).Name));
        foreach (var filter in filterList)
        {
            var allowedProperty = AllowedPropertyFilters.FirstOrDefault(_ => _.ParamKey == filter.Field);
            if (allowedProperty == null) continue;

            var propertyName = allowedProperty.PropertyName ?? allowedProperty.ParamKey;
            if (allowedProperty.RelationName != null && allowedProperty.PropertyName != null)
                propertyName = allowedProperty.RelationName;

            Console.WriteLine("Filter:\n");
            Console.WriteLine($"Filter prop\t: {filter.Field}");
            Console.WriteLine($"Filter operator\t: {filter.Operator}");
            Console.WriteLine($"Filter value\t: {filter.Value}");

            var prop = parentProperties.FirstOrDefault(e => e.Name.Equals(propertyName));
            if (prop == null) continue;

            Console.WriteLine("\nValid parent property:");
            Console.WriteLine($"-> {prop.Name}");

            var propType = prop.PropertyType;

            if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                Console.WriteLine($"\nDetect {prop.Name} is ICollection type");

                if (allowedProperty.PropertyName == null)
                    throw new Exception("Please init your property child name.");

                var childPropName   = allowedProperty.PropertyName;
                var childPropValue  = filter.Value;
                var childPropType   = propType.GetGenericArguments()[0];
                var childProperties = childPropType.GetProperties();
                var childProp       = childProperties.FirstOrDefault(p => p.Name == childPropName);

                if (childProp == null) continue;

                Console.WriteLine("\nFound child property:");
                Console.WriteLine($"Child class name\t: {childPropType.Name}");
                Console.WriteLine($"Child property name\t: {childPropName}");

                var childParameter = Expression.Parameter(childPropType, BuildLambdaParameterName(childPropType.Name));
                var childProperty  = Expression.Property(childParameter, childPropName);
                var childConstant  = Expression.Constant(childPropValue);

                var childMethod = filter.Operator switch
                {
                    OperatorsFilter.EqualOperator => childPropType.GetMethod("Equals", new[] { childPropType }),
                    _                             => throw new ArgumentException("Invalid child method.")
                };

                var childCall = Expression.Call(childProperty, childMethod!, childConstant);
                var anyExpression = Expression.Call(
                    typeof(Enumerable),
                    "Any",
                    new[] { childProp.DeclaringType }!,
                    Expression.Property(parentParameter, allowedProperty.RelationName!),
                    Expression.Lambda(childCall, childParameter)
                );

                // var lambdaParam = Expression.Parameter(childPropType, "x");
                // var lambdaBody  = BuildLambdaBody(childPropName, filter.Operator, childPropValue, lambdaParam);
                // if (lambdaBody == null) throw new Exception("Invalid configuration exception.");
                //
                // var lambdaExpr = Expression.Lambda(lambdaBody, lambdaParam);
                //
                // var anyMethod = typeof(Enumerable)
                //     .GetMethods()
                //     .Where(m => m.Name == "Any")
                //     .Single(m => m.GetParameters().Length == 2)
                //     .MakeGenericMethod(childPropType);

                // var whereExpr = Expression.Call(anyMethod,
                //     Expression.Property(parentLambdaParam, propertyName),
                //     lambdaExpr);

                var parentLambda = Expression.Lambda<Func<T, bool>>(anyExpression, parentParameter);

                Console.WriteLine("\nLambda expression:");
                Console.WriteLine($"{parentLambda}");

                filteredQuery = filteredQuery.Where(parentLambda);
            }
            else
            {
                switch (filter.Operator)
                {
                    case OperatorsFilter.EqualOperator:
                        filteredQuery = filteredQuery.Where($"{propertyName} == @0", filter.Value);
                        break;
                    case OperatorsFilter.NotEqualOperator:
                        filteredQuery = filteredQuery.Where($"{propertyName} != @0", filter.Value);
                        break;
                    case OperatorsFilter.LikeOperator:
                        filteredQuery = filteredQuery.Where($"{propertyName}.Contains(@0)", filter.Value);
                        break;
                    case OperatorsFilter.NotLikeOperator:
                        filteredQuery = filteredQuery.Where($"!{propertyName}.Contains(@0)", filter.Value);
                        break;
                    case OperatorsFilter.BetweenOperator:
                    {
                        object from  = null!;
                        object until = null!;
                        var allowBetween = new[]
                            {
                                typeof(DateTime),
                                typeof(double),
                                typeof(int),
                                typeof(long),
                                typeof(float),
                                typeof(decimal)
                            }
                            .Contains(prop.PropertyType);

                        if (allowBetween)
                        {
                            if (prop.PropertyType == typeof(DateTime))
                            {
                                from  = DateTime.Parse(((JArray)filter.Value)[0].ToString());
                                until = DateTime.Parse(((JArray)filter.Value)[1].ToString());
                            }
                            else
                            {
                                from  = Convert.ToDouble(((JArray)filter.Value)[0]);
                                until = Convert.ToDouble(((JArray)filter.Value)[1]);
                            }
                        }

                        filteredQuery = filteredQuery.Where($"{propertyName} >= @0 AND {filter.Field} <= @1", from,
                            until);
                        break;
                    }
                    case OperatorsFilter.LessThanOperator:
                    {
                        filteredQuery = filteredQuery.Where($"{propertyName} < @0", filter.Value);
                        break;
                    }
                    case OperatorsFilter.LessThanEqualOperator:
                    {
                        filteredQuery = filteredQuery.Where($"{propertyName} <= @0", filter.Value);
                        break;
                    }
                    case OperatorsFilter.GreaterThanOperator:
                    {
                        filteredQuery = filteredQuery.Where($"{propertyName} > @0", filter.Value);
                        break;
                    }
                    case OperatorsFilter.GreaterThanEqualOperator:
                    {
                        filteredQuery = filteredQuery.Where($"{propertyName} >= @0", filter.Value);
                        break;
                    }
                    case OperatorsFilter.InOperator:
                    {
                        filteredQuery = filteredQuery.Where($"{(JArray)filter.Value}.Contains(@0)", propertyName);
                        break;
                    }
                    case OperatorsFilter.NotInOperator:
                    {
                        filteredQuery = filteredQuery.Where($"!{(JArray)filter.Value}.Contains(@0)", propertyName);
                        break;
                    }
                    default: continue;
                }
            }
        }

        return filteredQuery;
    }

    private static string BuildLambdaParameterName(string name) => string.Concat(name.Where(char.IsUpper)).ToLower();

    // private static LambdaExpression BuildLambdaExpression<T>(
    //     string property,
    //     string operand,
    //     object value,
    //     ParameterExpression? childParam = null
    // )
    // {
    //     try
    //     {
    //         var parameter = Expression.Parameter(typeof(T), "x");
    //         var member    = Expression.Property(parameter, property);
    //
    //         MethodCallExpression? call = null;
    //
    //         switch (operand)
    //         {
    //             case OperatorsFilter.EqualOperator:
    //                 var method   = property.GetType().GetMethod("Equals", new[] { property.GetType() });
    //                 var constant = Expression.Constant(value);
    //                 call = Expression.Call(member, method, constant);
    //                 break;
    //         }
    //
    //         return childParam is null
    //             ? Expression.Lambda<Func<T, bool>>()
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine($"Error when build lambda\nErrors: {e.Message}");
    //         throw;
    //     }
    // }
}