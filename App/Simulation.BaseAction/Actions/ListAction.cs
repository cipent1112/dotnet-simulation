using System.Collections;
using System.Reflection;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Simulation.BaseAction.Constants;
using Simulation.BaseAction.Filters;

namespace Simulation.BaseAction.Actions;

public class ListAction
{
    public ListAction(List<AllowedFilter>? allowedFilters = null)
    {
        AllowedFilters = allowedFilters ?? null;
    }

    public List<AllowedFilter>? AllowedFilters { get; set; }

    public IQueryable<T> ApplyFilter<T>(IQueryable<T> queryable, List<QueryParam> queryParams)
    {
        var query          = queryable;
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

            var filterValues     = new List<string>();
            var generatedFilters = new List<string[]>();

            var relationTypes = GetRelationTypes<T>(relations);

            foreach (var filter in filters)
            {
                var filterProperty = filter.Property;

                string? filterValue = null;

                if (filter.FilterValue != null) filterValue = filter.FilterValue;
                if (filter.FilterKey != null) filterValue   = queryParams.FirstOrDefault(_ => _.Field == filter.FilterKey)?.Value?.ToString();

                if (filterValue == null) continue;

                filterValues.Add(filterValue);

                Console.WriteLine(JsonConvert.SerializeObject(relationTypes));

                generatedFilters.Add(new[] { filter.Conjunction, filterProperty, filter.FilterOperand, filterValue });
            }

            var condition = BuildCondition(relationTypes, relations, generatedFilters);

            Console.WriteLine(JsonConvert.SerializeObject(generatedFilters));
            Console.WriteLine(string.Join(", ", filterValues));
        }

        return query;
    }


    private static int GetRelationType(Type propertyType)
    {
        return propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
            ? 2
            : 1;
    }

    private static List<int> GetRelationTypes<TEntity>(IReadOnlyList<string>? relations)
    {
        if (relations == null) return new List<int> { 0 };

        var parentClass   = typeof(TEntity);
        var relationTypes = new List<int>();

        var           previousClass = parentClass;
        PropertyInfo? propertyInfo;

        for (var i = 0; i < relations.Count; i++)
        {
            propertyInfo = previousClass.GetProperty(relations[i]);
            if (propertyInfo == null) continue;

            var relationType = GetRelationType(propertyInfo.PropertyType);

            relationTypes.Add(relationType);
            previousClass = relationType == 2 ? propertyInfo.PropertyType.GetGenericArguments()[0] : propertyInfo.PropertyType;
        }


        return relationTypes;
    }

    private static string BuildCondition(IReadOnlyList<int> relationTypes, List<string>? relations, List<string[]> filters)
    {
        var filterConditions = string.Empty;

        var index = 0;
        foreach (var filter in filters)
        {
            string filterCondition;

            var conjunction = index == 0 ? String.Empty : filter[0];
            var property    = filter[1];
            var operand     = filter[2];
            var value       = filter[3];

            if (new List<string>
                {
                    Operand.EqualOperator, Operand.NotEqualOperator,
                    Operand.LessThanOperator, Operand.LessThanEqualOperator,
                    Operand.GreaterThanOperator, Operand.GreaterThanEqualOperator
                }.Contains(operand))
            {
                filterCondition = $"{property} {operand} @{index}";
            }
            else if (new List<string> { Operand.LikeOperator }.Contains(operand))
            {
                filterCondition = $"{property}.Contains(@{index})";
            }
            else if (new List<string> { Operand.NotLikeOperator }.Contains(operand))
            {
                filterCondition = $"!{property}.Contains(@{index})";
            }
            else if (operand.Equals(Operand.BetweenOperator))
            {
                filterCondition = $"{property} >= @{index} && {property} <= @{index + 1}";
                index++;
            }
            else throw new Exception("Conjunction not supported");

            filterConditions += conjunction + filterCondition;

            index++;
        }

        if (!(relations?.Count >= 1)) return filterConditions;

        for (var iC = relationTypes.Count - 1; iC >= 0; iC--)
        {
            filterConditions = relationTypes[iC] == 2
                ? $"{relations[iC]}.Any({filterConditions})"
                : $"{relations[iC]}.{filterConditions}";
        }

        Console.WriteLine(filterConditions);
        return filterConditions;
    }

}