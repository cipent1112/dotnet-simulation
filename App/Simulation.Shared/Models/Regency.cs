namespace Simulation.Shared.Models;

public class Regency
{
    public const string StatusActive   = "Active";
    public const string StatusInactive = "Inactive";

    public Regency()
    {
        Id = Guid.NewGuid().ToString().ToLower();
    }

    public string Id         { get; set; }
    public string ProvinceId { get; set; } = null!;
    public string Code       { get; set; } = null!;
    public string Name       { get; set; } = null!;
    public string Status     { get; set; } = null!;

    public virtual Province              Province  { get; set; } = null!;
    public virtual ICollection<District> Districts { get; set; } = null!;
}