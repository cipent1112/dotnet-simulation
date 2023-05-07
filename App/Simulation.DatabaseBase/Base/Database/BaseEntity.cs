using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simulation.DatabaseBase.Base.Database;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
// ReSharper disable MemberCanBePrivate.Global
public abstract class BaseEntity
{
    protected bool IsSoftDelete;
    public static string GetTableName<T>(T name) where T : class => nameof(T);

    protected BaseEntity()
    {
        var guid = new Guid();
        Id = guid.ToString().ToLower();
    }

    [Key]
    [Column(Order = 0)]
    [StringLength(36)]
    public string Id { get; set; }

    [StringLength(36)]
    [Column(Order = 95)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime", Order = 96)]
    public DateTime CreatedAt { get; set; }

    [StringLength(36)]
    [Column(Order = 97)]
    public string? UpdatedBy { get; set; }

    [Column(TypeName = "datetime", Order = 98)]
    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    [StringLength(36)]
    [Column(Order = 99)]
    public string? DeletedBy { get; set; }

    [Column(TypeName = "datetime", Order = 100)]
    public DateTime? DeletedAt { get; set; }
}