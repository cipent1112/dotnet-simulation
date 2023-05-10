namespace Simulation.Shared.Models;

public class Province
{
    public const string StatusActive   = "Active";
    public const string StatusInactive = "Inactive";

    public string Id     { get; set; } = null!;
    public string Name   { get; set; } = null!;
    public string Status { get; set; } = null!;

    public virtual ICollection<ProvinceAssignment> ProvinceAssignments { get; set; } = null!;
    public virtual ICollection<Regency>            Regencies           { get; set; } = null!;
}