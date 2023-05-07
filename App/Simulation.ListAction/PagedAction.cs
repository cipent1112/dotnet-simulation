using System.Linq.Expressions;
using Newtonsoft.Json;
using Simulation.Shared;

namespace Simulation.ListAction;

public class PagedAction
{
    public List<AllowedPropertyFilter>? AllowedPropertyFilters { get; set; }

    public PagedAction(List<AllowedPropertyFilter>? allowedPropertyFilters)
    {
        AllowedPropertyFilters = allowedPropertyFilters;
    }

    public IQueryable<T> ApplyFilters<T>(IQueryable<T> query, List<PropertiesFilter> filterList)
    {
        var filteredQuery     = query;
        var parentProperties  = typeof(T).GetProperties();
        var parentLambdaParam = Expression.Parameter(typeof(T), "x");

        Console.WriteLine();
        Console.WriteLine($"{typeof(T).Name} properties:");
        parentProperties.ToList().ForEach(_ => Console.WriteLine($"-> {_.Name}"));

        Console.WriteLine();
        Console.WriteLine("Allowed Property Filters: ");
        AllowedPropertyFilters!.ForEach(_ =>
        {
            Console.WriteLine($"Param Key\t\t: {_.ParamKey}");
            Console.WriteLine($"Parent Property Name\t: {_.PropertyName}");
            Console.WriteLine($"Child Property Name\t: {_.ChildPropertyName}");
        });
        Console.WriteLine();

        foreach (var filter in filterList)
        {
            var parentPropName  = filter.Field;
            var allowedProperty = AllowedPropertyFilters?.FirstOrDefault(_ => _.ParamKey == parentPropName);
            if (allowedProperty != null)
            {
                parentPropName = allowedProperty.ParamKey;
                if (!string.IsNullOrEmpty(allowedProperty.PropertyName))
                    parentPropName = allowedProperty.PropertyName;
            }

            Console.WriteLine("\nFilter:");
            Console.WriteLine($"Filter prop\t: {parentPropName}");
            Console.WriteLine($"Filter operator\t: {filter.Operator}");
            Console.WriteLine($"Filter value\t: {filter.Value}");

            var prop = parentProperties.FirstOrDefault(e => e.Name.Equals(parentPropName));
            if (prop == null) continue;

            Console.WriteLine("\nValid parent property:");
            Console.WriteLine($"-> {prop.Name}");

            var propType = prop.PropertyType;
            if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                Console.WriteLine($"\nDetect {prop.Name} is ICollection type");

                if (allowedProperty?.ChildPropertyName == null)
                    throw new Exception("Please init your property child name.");

                var childPropName   = allowedProperty.ChildPropertyName;
                var childPropValue  = filter.Value;
                var childPropType   = propType.GetGenericArguments()[0];
                var childProperties = childPropType.GetProperties();
                var childProp       = childProperties.FirstOrDefault(p => p.Name == childPropName);

                if (childProp == null) continue;

                Console.WriteLine("\nValid child property:");
                Console.WriteLine($"Child class name\t: {childPropType.Name}");
                Console.WriteLine($"Child property name\t: {childPropName}");

                var lambdaParam = Expression.Parameter(typeof(StoreAssignment), "x");
                var lambdaBody = Expression.Equal(
                    Expression.Property(lambdaParam, childPropName),
                    Expression.Constant(childPropValue)
                );
                var lambdaExpr = Expression.Lambda<Func<StoreAssignment, bool>>(lambdaBody, lambdaParam);

                var anyMethod = typeof(Enumerable)
                    .GetMethods()
                    .Where(m => m.Name == "Any")
                    .Single(m => m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(StoreAssignment));

                var whereExpr = Expression.Call(anyMethod,
                    Expression.Property(parentLambdaParam, parentPropName),
                    lambdaExpr);

                var lambda = Expression.Lambda<Func<T, bool>>(whereExpr, parentLambdaParam);

                Console.WriteLine();
                Console.WriteLine($"{lambdaParam}");
                Console.WriteLine($"{lambdaBody}");
                Console.WriteLine($"{lambdaExpr}");
                Console.WriteLine($"{whereExpr}");


                filteredQuery = filteredQuery.Where(lambda);
            }
        }

        // foreach (var filter in filterList)
        // {
        //     var parentPropName = filter.Field;
        //
        //     /* check the filter field key is in allowed property param key */
        //     var allowedProperty = AllowedPropertyFilters?.FirstOrDefault(_ => _.ParamKey == parentPropName);
        //     if (allowedProperty != null)
        //     {
        //         /* when allowed property is not null -> override parentPropName to allowed param key */
        //         parentPropName = allowedProperty.ParamKey;
        //
        //         /* when allowed property name is not null -> override parentPropName to allowed property name */
        //         if (!string.IsNullOrEmpty(allowedProperty.PropertyName))
        //             parentPropName = allowedProperty.PropertyName;
        //     }
        //
        //     /* check the propertyName is exist in parent property class */
        //     var prop = entityProperties.FirstOrDefault(e => e.Name.Equals(parentPropName));
        //     if (prop == null)
        //     {
        //         Console.ForegroundColor = ConsoleColor.Yellow;
        //         Console.WriteLine($"Property {parentPropName} is not exist in {typeof(T).Name} class.");
        //         Console.ResetColor();
        //         continue;
        //     }
        //
        //     var propType = prop.PropertyType;
        //
        //     /* check the property type in parent class is reference to nested child using ICollection type. */
        //     if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(ICollection<>))
        //     {
        //         if (allowedProperty?.ChildPropertyName == null)
        //             throw new Exception("Please init your property child name.");
        //
        //         // If the property is an ICollection, check if it contains the specified value
        //         var childPropName   = allowedProperty.ChildPropertyName;
        //         var childPropValue  = filter.Value;
        //         var childPropType   = propType.GetGenericArguments()[0];
        //         var childProperties = childPropType.GetProperties();
        //
        //         var childProp = childProperties.FirstOrDefault(p => p.Name == childPropName);
        //         if (childProp == null)
        //         {
        //             Console.ForegroundColor = ConsoleColor.Yellow;
        //             Console.WriteLine($"Property {childPropName} is not exist.");
        //             Console.ResetColor();
        //             continue;
        //         }
        //
        //         var lambdaParam = Expression.Parameter(childPropType, "c");
        //         var memberExpr  = Expression.Property(lambdaParam, childProp);
        //         var lambdaBody  = Expression.Equal(memberExpr, Expression.Constant(childPropValue));
        //         var lambdaExpr  = Expression.Lambda<Func<object, bool>>(lambdaBody, lambdaParam);
        //
        //         var anyMethod = typeof(Enumerable)
        //             .GetMethods()
        //             .Where(m => m.Name == "Any")
        //             .Single(m => m.GetParameters().Length == 2)
        //             .MakeGenericMethod(childPropType);
        //
        //         var whereExpr = Expression.Call(
        //             anyMethod,
        //             Expression.Property(memberExpr, "AsQueryable"),
        //             lambdaExpr
        //         );
        //
        //         filteredQuery = filteredQuery.Provider.CreateQuery<T>(
        //             Expression.Call(
        //                 typeof(Queryable),
        //                 "Where",
        //                 new[] { typeof(T) },
        //                 filteredQuery.Expression,
        //                 Expression.Lambda<Func<T, bool>>(whereExpr, lambdaExpr.Parameters.Single())
        //             )
        //         );
        //     }
        //     else
        //     {
        //         // If the property is not an ICollection, apply the filter normally
        //         switch (filter.Operator)
        //         {
        //             case OperatorsFilter.EqualOperator:
        //                 filteredQuery = filteredQuery.Where($"{filter.Field} == @0", filter.Value);
        //                 break;
        //             case OperatorsFilter.NotEqualOperator:
        //                 filteredQuery = filteredQuery.Where($"{filter.Field} != @0", filter.Value);
        //                 break;
        //             case OperatorsFilter.LikeOperator:
        //                 filteredQuery = filteredQuery.Where($"{filter.Field}.Contains(@0)", filter.Value);
        //                 break;
        //             default:
        //                 continue;
        //         }
        //     }
        // }

        return filteredQuery;
    }
}