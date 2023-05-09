namespace Simulation.Shared.Models;

public class Product
{
    public string Id          { get; set; } = null!;
    public string StoreId     { get; set; } = null!;
    public string Name        { get; set; } = null!;
    public string Description { get; set; } = null!;

    public Store                     Store         { get; set; } = null!;
    public ICollection<ProductImage> ProductImages { get; set; } = null!;
}