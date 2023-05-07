using Microsoft.EntityFrameworkCore;
using Simulation.DatabaseBase.Base.Database;
using Simulation.DatabaseBase.Common.Models.Location;

namespace Simulation.DatabaseBase.Data.Contexts;

public sealed class LocationDbContext : BaseDbContext
{
    public DbSet<Province> Province { get; set; } = null!;

    public LocationDbContext(DbContextOptions<LocationDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
}