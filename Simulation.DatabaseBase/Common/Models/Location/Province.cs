using Simulation.DatabaseBase.Base.Database;

namespace Simulation.DatabaseBase.Common.Models.Location;

public class Province : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Status { get; set; } = null!;
}