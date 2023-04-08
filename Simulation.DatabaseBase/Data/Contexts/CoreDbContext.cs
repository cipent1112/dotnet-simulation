using Microsoft.EntityFrameworkCore;
using Simulation.DatabaseBase.Base.Database;
using Simulation.DatabaseBase.Common.Models.Core;

namespace Simulation.DatabaseBase.Data.Contexts;

public sealed class CoreDbContext : BaseDbContext<CoreDbContext>
{
    public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options)
    {
    }

    public DbSet<Author> Author { get; set; } = null!;
    public DbSet<Author> Book { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);
        mb.UseCollation("SQL_Latin1_General_CP1_CI_AS");
    }
}