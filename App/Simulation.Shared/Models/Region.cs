namespace Simulation.Shared.Models;

public class Region
{
    public const string StatusActive   = "Active";
    public const string StatusInactive = "Inactive";

    public Region()
    {
        Id = Guid.NewGuid().ToString().ToLower();
    }

    public string   Id        { get; set; }
    public string   Name      { get; set; } = null!;
    public string   Status    { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public ICollection<RegionProvince> RegionProvinces { get; set; } = null!;
}