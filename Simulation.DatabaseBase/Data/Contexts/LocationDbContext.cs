using Microsoft.EntityFrameworkCore;
using Simulation.DatabaseBase.Base.Database;
using Simulation.DatabaseBase.Common.Models.Location;

namespace Simulation.DatabaseBase.Data.Contexts;

public class LocationDbContext : BaseDbContext<LocationDbContext>
{

    public DbSet<Province> Province { get; set; } = null!;

    public LocationDbContext(DbContextOptions<LocationDbContext> dbSetting) : base(dbSetting)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);
        mb.UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}