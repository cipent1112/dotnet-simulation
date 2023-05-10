﻿namespace Simulation.Shared.Models;

public class Village
{
    public const string StatusActive   = "Active";
    public const string StatusInactive = "Inactive";

    public string Id         { get; set; } = null!;
    public string DistrictId { get; set; } = null!;
    public string Name       { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Status     { get; set; } = null!;

    public virtual District District { get; set; } = null!;
}