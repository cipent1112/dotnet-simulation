namespace Simulation.Shared.Models;

public class RegionProvince
{
    public const string StatusActive   = "Active";
    public const string StatusInactive = "Inactive";

    public RegionProvince()
    {
        Id = Guid.NewGuid().ToString().ToLower();
    }

    public string   Id         { get; set; }
    public string   RegionId   { get; set; } = null!;
    public string   ProvinceId { get; set; } = null!;
    public string   Status     { get; set; } = null!;
    public DateTime CreatedAt  { get; set; }

    public Region   Region   { get; set; }
    public Province Province { get; set; }
}