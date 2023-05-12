namespace Simulation.Shared.Models;

public class Province
{
    public const string StatusActive   = "Active";
    public const string StatusInactive = "Inactive";

    public Province()
    {
        Id = Guid.NewGuid().ToString().ToLower();
    }

    public string Id     { get; set; }
    public string Name   { get; set; } = null!;
    public string Status { get; set; } = null!;

    public virtual ICollection<ProvinceAssignment> ProvinceAssignments { get; set; } = null!;
    public virtual ICollection<Regency>            Regencies           { get; set; } = null!;
    public virtual ICollection<RegionProvince>     RegionProvinces     { get; set; } = null!;
}