using System.ComponentModel.DataAnnotations.Schema;

namespace Simulation.PaginationFilters.Models;

public class Product
{
    public const string StatusAvailable = "Available";
    public const string StatusNotAvailable = "NotAvailable";
    public const string StatusOutOfStock = "OutOfStock";

    [NotMapped]
    public static Dictionary<string, string> Statuses => new()
    {
        { StatusAvailable, "Available" },
        { StatusNotAvailable, "Not Available" },
        { StatusOutOfStock, "Out of Stock" }
    };

    public Product()
    {
        var guid = Guid.NewGuid();
        Id = guid.ToString().ToLower();
    }

    #region product properties

    public string Id { get; set; }
    public string StoreId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public double Price { get; set; }
    public int Stock { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public Store Store { get; set; } = null!;

    #endregion
}