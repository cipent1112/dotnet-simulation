using Microsoft.EntityFrameworkCore;

// ReSharper disable VirtualMemberCallInConstructor

namespace Simulation.DatabaseBase.Base.Database;

public class BaseDbContext<T> : DbContext where T : DbContext
{
    public BaseDbContext(DbContextOptions<T> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<BaseEntity>().HasQueryFilter(b => !b.IsDeleted);
    }

    public override int SaveChanges()
    {
        ChangeTracker.Entries<BaseEntity>().DatetimeBehavior("CreatedAt", "UpdatedAt");
        ChangeTracker.Entries<BaseEntity>().OwnerBehavior("CreatedBy", "UpdatedBy");
        ChangeTracker.Entries<BaseEntity>().SoftDeleteBehavior("IsDeleted", "DeletedBy", "DeletedAt");
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = new())
    {
        ChangeTracker.Entries<BaseEntity>().DatetimeBehavior("CreatedAt", "UpdatedAt");
        ChangeTracker.Entries<BaseEntity>().OwnerBehavior("CreatedBy", "UpdatedBy");
        ChangeTracker.Entries<BaseEntity>().SoftDeleteBehavior("IsDeleted", "DeletedBy", "DeletedAt");
        return await base.SaveChangesAsync(ct);
    }
}