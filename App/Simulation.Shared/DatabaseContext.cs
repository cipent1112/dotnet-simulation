using Microsoft.EntityFrameworkCore;
using Simulation.Shared.Models;

namespace Simulation.Shared;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<Store>           Store           { get; set; } = null!;
    public DbSet<StoreAssignment> StoreAssignment { get; set; } = null!;
    public DbSet<Location>        Location        { get; set; } = null!;
    public DbSet<Product>         Product         { get; set; } = null!;
    public DbSet<ProductImage>    ProductImage    { get; set; } = null!;
    public DbSet<Province>        Province        { get; set; } = null!;
    public DbSet<Regency>         Regency         { get; set; } = null!;
    public DbSet<District>        District        { get; set; } = null!;
    public DbSet<Village>         Village         { get; set; } = null!;
}