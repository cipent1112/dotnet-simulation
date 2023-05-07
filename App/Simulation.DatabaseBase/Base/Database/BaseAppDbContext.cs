using Microsoft.EntityFrameworkCore;
using Simulation.DatabaseBase.Base.Models;
using Entities = Simulation.DatabaseBase.Base.Models;

namespace Simulation.DatabaseBase.Base.Database;

public class BaseAppDbContext : BaseDbContext
{
    public DbSet<Client> Client { get; set; } = null!;
    public DbSet<ClientWhitelist> ClientWhitelist { get; set; } = null!;

    protected BaseAppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected BaseAppDbContext(DbContextOptions<BaseAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);
        
        mb.Entity<Client>(e => e.Property(c => c.Status).HasDefaultValueSql($"('{Entities.Client.StatusActive}')"));
        mb.Entity<ClientWhitelist>(e =>
        {
            e.Property(c => c.Status).HasDefaultValueSql($"('{Entities.ClientWhitelist.StatusActive}')");
            e.HasOne(c => c.Client)
                .WithMany(p => p.ClientWhitelists)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ForeignKeyClientClientWhitelist");
        });
    }
}