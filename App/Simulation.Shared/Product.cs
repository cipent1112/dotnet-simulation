namespace Simulation.Shared;

public class Product
{
    public Product() => Id = Guid.NewGuid().ToString().ToLower();

    public string Id          { get; set; }
    public string StoreId     { get; set; } = null!;
    public string Name        { get; set; } = null!;
    public string Description { get; set; } = null!;

    public Store Store { get; set; } = null!;
}