namespace Simulation.Shared;

public class Location
{
    public Location() => Id = Guid.NewGuid().ToString().ToLower();

    public string Id      { get; set; }
    public string StoreId { get; set; } = null!;
    public string City    { get; set; } = null!;
    public string Address { get; set; } = null!;

    public Store Store { get; set; } = null!;
}