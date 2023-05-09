namespace Simulation.Shared.Models;

public class StoreAssignment
{
    public const string AssignmentStatusNew      = "New";
    public const string AssignmentStatusApproved = "Approved";
    public const string AssignmentStatusRejected = "Rejected";

    public const string StatusActive   = "Active";
    public const string StatusInactive = "Inactive";

    public StoreAssignment() => Id = Guid.NewGuid().ToString().ToLower();

    public string Id               { get; set; }
    public string StoreId          { get; set; } = null!;
    public string AssignmentStatus { get; set; } = null!;
    public string Status           { get; set; } = null!;
    public Store  Store            { get; set; } = null!;
}