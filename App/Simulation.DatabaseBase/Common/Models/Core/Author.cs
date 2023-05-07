using Simulation.DatabaseBase.Base.Database;

namespace Simulation.DatabaseBase.Common.Models.Core;

public class Author : BaseEntity
{
    public const string StatusActive = "Active";
    public const string StatusInactive = "Inactive";
    
    public string Name { get; set; } = null!;
    public int Age { get; set; }
    public string Status { get; set; } = null!;
}