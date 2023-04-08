using Simulation.DatabaseBase.Base.Database;

namespace Simulation.DatabaseBase.Common.Models.Core;

public class Book : BaseEntity
{
    public const string StatusAvailable = "Available";
    public const string StatusPreOrder = "PreOrder";
    public const string StatusOutOfStock = "OutOfStock";
    
    public string AuthorId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }
    public string Status { get; set; } = null!;
}