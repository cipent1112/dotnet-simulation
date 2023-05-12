using System.Collections;
using System.Reflection;
using Simulation.BaseAction.Filters;

namespace Simulation.BaseAction.Actions;

public class ListAction<T> where T : class
{
    public ListAction(IQueryable<T> queryable, List<AllowedFilter>? allowedFilters = null)
    {
        Queryable      = queryable;
        AllowedFilters = allowedFilters ?? null;
    }

    public IQueryable<T>        Queryable      { get; set; }
    public List<AllowedFilter>? AllowedFilters { get; set; }

    public IEnumerable<T> ApplyFilter(List<QueryParam> queryParams)
    {
        var query          = Queryable;
        var allowedFilters = AllowedFilters;

        if (allowedFilters == null) return query;

        var operands = typeof(QueryParam)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(field => field.FieldType == typeof(string) && field.IsLiteral && !field.IsInitOnly)
            .Select(field => field.GetValue(null)?.ToString())
            .ToArray();

        foreach (var allowedFilter in allowedFilters)
        {
            var relations = allowedFilter.Relations;
            var filters   = allowedFilter.Filters;

            foreach (var filter in filters)
            {
                var filterProperty = filter.Property;
                Console.WriteLine(GetRelationModes<T>(relations, filterProperty));
            }
        }


        return query;
    }


    private static int GetMode(Type propertyType)
    {
        return propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
            ? 2
            : 1;
    }

    private static int[] GetRelationModes<TEntity>(IReadOnlyList<string> relations, string property)
    {
        var parentClass = typeof(TEntity);
        var modes       = new int[relations.Count];

        var previousClass = parentClass;
        var propertyInfo  = parentClass.GetProperty(property);

        for (var i = 0; i < relations.Count; i++)
        {
            propertyInfo = previousClass.GetProperty(relations[i]);
            if (propertyInfo == null) continue;

            modes[i] = GetMode(propertyInfo.PropertyType);
            previousClass = modes[i] == 2
                ? propertyInfo.PropertyType.GetGenericArguments()[0]
                : propertyInfo.PropertyType;
        }


        return modes;
    }
}