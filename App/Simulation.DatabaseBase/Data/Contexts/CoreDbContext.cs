using Microsoft.EntityFrameworkCore;
using Simulation.DatabaseBase.Base.Database;
using Simulation.DatabaseBase.Common.Models.Core;

namespace Simulation.DatabaseBase.Data.Contexts;

public sealed class CoreDbContext : BaseAppDbContext
{
    public DbSet<Author> Author { get; set; } = null!;
    public DbSet<Book> Book { get; set; } = null!;

    public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

}