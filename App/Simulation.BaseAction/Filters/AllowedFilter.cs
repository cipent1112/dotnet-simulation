using System.Reflection;

namespace Simulation.BaseAction.Filters;

public class AllowedFilter
{
    public List<string>? Relations { get; set; }
    public List<Filter> Filters   { get; set; } = null!;
    
    public List<int> GetRelationTypes<TEntity>()
    {
        if (Relations == null) return new List<int> { 0 };

        var parentClass   = typeof(TEntity);
        var relationTypes = new List<int>();

        var           previousClass = parentClass;
        PropertyInfo? propertyInfo;

        foreach (var relation in Relations)
        {
            propertyInfo = previousClass.GetProperty(relation);
            if (propertyInfo == null) continue;

            var relationType = GetRelationType(propertyInfo.PropertyType);

            relationTypes.Add(relationType);
            previousClass = relationType == 2 ? propertyInfo.PropertyType.GetGenericArguments()[0] : propertyInfo.PropertyType;
        }


        return relationTypes;
    }
    
    private static int GetRelationType(Type propertyType)
    {
        return propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>) ? 2 : 1;
    }
}