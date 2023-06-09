﻿namespace Simulation.Shared.Models;

public class District
{
    public const string StatusActive   = "Active";
    public const string StatusInactive = "Inactive";

    public District()
    {
        Id = Guid.NewGuid().ToString().ToLower();
    }

    public string Id        { get; set; }
    public string RegencyId { get; set; } = null!;
    public string Name      { get; set; } = null!;
    public string Status    { get; set; } = null!;

    public virtual Regency              Regency  { get; set; } = null!;
    public virtual ICollection<Village> Villages { get; set; } = null!;
}