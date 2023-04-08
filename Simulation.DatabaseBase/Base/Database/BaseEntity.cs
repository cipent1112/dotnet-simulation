using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simulation.DatabaseBase.Base.Database;

public class BaseEntity
{
    [NotMapped] protected bool IsSoftDelete { get; init; } = true;
    
    public BaseEntity()
    {
        var guid = new Guid();
        Id = guid.ToString().ToLower();
    }
    
    [Key]
    [StringLength(36)]
    public string Id { get; set; }
    
    [StringLength(36)]
    public string? CreatedBy { get; set; }
    
    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
    
    [StringLength(36)]
    public string? UpdatedBy { get; set; }
    
    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }
    
    public bool IsDeleted { get; set; }
    
    [StringLength(36)]
    public string? DeletedBy { get; set; }
    
    [Column(TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }
}