// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations.Schema;

namespace Simulation.Shared;

public class Store
{
    public string                       Id               { get; set; } = null!;
    public string                       Name             { get; set; } = null!;
    public ICollection<StoreAssignment> StoreAssignments { get; set; } = null!;
    public ICollection<Product>         Products         { get; set; } = null!;
    public ICollection<Location>        Locations        { get; set; } = null!;

    [NotMapped]
    public StoreAssignment? CurrentAssignment => StoreAssignments
        .Where(_ => _.StoreId == Id)
        .FirstOrDefault(_ => _.Status is StoreAssignment.StatusActive);
}