using System.ComponentModel.DataAnnotations.Schema;

namespace Simulation.PaginationFilters.Models;

public class Store
{
    public const string StatusActive = "Active";
    public const string StatusInActive = "Inactive";

    [NotMapped]
    public static Dictionary<string, string> Statuses => new()
    {
        { StatusActive, $"{nameof(Store)} is `Active`" },
        { StatusInActive, $"{nameof(Store)} is `Inactive`" }
    };

    public Store()
    {
        var guid = Guid.NewGuid();
        Id = guid.ToString().ToLower();
        Products = new HashSet<Product>();
    }
    
    #region store properties

    public string Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Address { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public ICollection<Product> Products { get; set; }

    #endregion
}