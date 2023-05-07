namespace Simulation.Shared;

public class ProductImage
{
    public ProductImage() => Id = Guid.NewGuid().ToString().ToLower();

    public string Id        { get; set; }
    public string ProductId { get; set; } = null!;
    public string Url       { get; set; } = null!;

    public Product Product { get; set; } = null!;
}