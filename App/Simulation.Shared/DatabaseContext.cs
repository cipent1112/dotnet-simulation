using Microsoft.EntityFrameworkCore;
using Simulation.Shared.Models;

namespace Simulation.Shared;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<Region>             Region             { get; set; } = null!;
    public DbSet<RegionProvince>     RegionProvince     { get; set; } = null!;
    public DbSet<Province>           Province           { get; set; } = null!;
    public DbSet<ProvinceAssignment> ProvinceAssignment { get; set; } = null!;
    public DbSet<Regency>            Regency            { get; set; } = null!;
    public DbSet<District>           District           { get; set; } = null!;
    public DbSet<Village>            Village            { get; set; } = null!;
}