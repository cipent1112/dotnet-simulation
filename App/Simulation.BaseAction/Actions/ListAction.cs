using System.Collections;
using System.Linq.Dynamic.Core;
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

    private List<AllowedFilter>? AllowedFilters { get; set; }

    public IQueryable<T> ApplyFilter<T>(IQueryable<T> queryable, List<QueryParam> queryParams)
    {
        var query          = queryable;
        var allowedFilters = AllowedFilters;

        if (allowedFilters == null) return query;

        foreach (var allowedFilter in allowedFilters) {
            var relations = allowedFilter.Relations;
            var filters   = allowedFilter.Filters;

            var filterValues     = new List<object>();
            var formattedFilters = new List<string[]>();

            var relationTypes = GetRelationTypes<T>(relations);

            foreach (var filter in filters) {
                var filterQueryParam = queryParams.FirstOrDefault(_ => _.Field == filter.FilterKey);
                if (filterQueryParam == null) continue;

                var filterValue   = filter.GetFilterValue(filterQueryParam);
                var filterOperand = Filter.GetFilterOperand(filterQueryParam);

                if (filterValue == null) continue;

                if (filterOperand == Operand.BetweenOperator)
                    filterValues.AddRange(((IEnumerable)filterValue).Cast<object?>()!);
                else filterValues.Add(filterValue);

                formattedFilters.Add(new[] { filter.Conjunction, filter.Property, filterOperand });
            }

            if (filterValues.Count <= 0) continue;

            Console.WriteLine($"FILTER VALUES: {JsonConvert.SerializeObject(filterValues.ToDynamicArray())}");

            var condition = BuildCondition(relationTypes, relations, formattedFilters);
            query = query.Where(string.Format(condition), filterValues.ToDynamicArray());
        }

        return query;
    }


    private static int GetRelationType(Type propertyType)
    {
        return propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>) ? 2 : 1;
    }

    private static List<int> GetRelationTypes<TEntity>(IReadOnlyList<string>? relations)
    {
        if (relations == null) return new List<int> { 0 };

        var parentClass   = typeof(TEntity);
        var relationTypes = new List<int>();
        var previousClass = parentClass;

        foreach (var relation in relations) {
            var propertyInfo = previousClass.GetProperty(relation);
            if (propertyInfo == null) continue;

            var relationType = GetRelationType(propertyInfo.PropertyType);

            relationTypes.Add(relationType);
            previousClass = relationType == 2
                ? propertyInfo.PropertyType.GetGenericArguments()[0]
                : propertyInfo.PropertyType;
        }


        return relationTypes;
    }

    private static string BuildCondition(IReadOnlyList<int> relationTypes, IReadOnlyList<string>? relations,
        List<string[]> filters)
    {
        var filterConditions = string.Empty;

        var index = 0;
        foreach (var filter in filters) {
            var conjunction = index == 0 ? string.Empty : $" {filter[0]} ";
            filterConditions += conjunction + BuildPropertyCondition(filter[1], filter[2], ref index);
        }

        if (relations?.Count > 0) {
            for (var iC = relationTypes.Count - 1; iC >= 0; iC--) {
                filterConditions = relationTypes[iC] == 2
                    ? $"{relations[iC]}.Any({filterConditions})"
                    : $"{relations[iC]}.{filterConditions}";
            }
        }

        Console.WriteLine(filterConditions);
        return filterConditions;
    }

    private static string BuildPropertyCondition(string property, string operand, ref int index)
    {
        var currentIndex = index;
        index++;

        if (new List<string>
            {
                Operand.EqualOperator, Operand.NotEqualOperator,
                Operand.LessThanOperator, Operand.LessThanEqualOperator,
                Operand.GreaterThanOperator, Operand.GreaterThanEqualOperator
            }.Contains(operand)) return $"{property} {operand} @{currentIndex}";

        switch (operand) {
            case Operand.LikeOperator:
                return $"{property}.Contains(@{currentIndex})";
            case Operand.NotLikeOperator:
                return $"!{property}.Contains(@{currentIndex})";
            case Operand.BetweenOperator:
                index++;
                return $"{property} >= @{currentIndex} && {property} <= @{currentIndex + 1}";
            default:
                throw new Exception("Conjunction not supported");
        }
    }
}