namespace Simulation.Shared.Models;

public class Village
{
    public const string StatusActive   = "Active";
    public const string StatusInactive = "Inactive";

    public Village()
    {
        Id = Guid.NewGuid().ToString().ToLower();
    }
    
    public string Id         { get; set; }
    public string DistrictId { get; set; } = null!;
    public string Name       { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Status     { get; set; } = null!;

    public virtual District District { get; set; } = null!;
}