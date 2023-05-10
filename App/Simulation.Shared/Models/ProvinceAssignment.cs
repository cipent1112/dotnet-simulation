namespace Simulation.Shared.Models;

public class ProvinceAssignment
{
    public const string StatusActive   = "Active";
    public const string StatusInactive = "Inactive";

    public const string AssignmentStatusNew      = "New";
    public const string AssignmentStatusApproved = "Approved";
    public const string AssignmentStatusRejected = "Rejected";

    public string Id               { get; set; } = null!;
    public string ProvinceId       { get; set; } = null!;
    public string AssignmentStatus { get; set; } = null!;
    public string Status           { get; set; } = null!;

    public Province Province { get; set; } = null!;
}