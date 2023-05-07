using Microsoft.EntityFrameworkCore;

namespace Simulation.Shared;

public class MockRepository
{
    private readonly List<Store> _stores;

    public MockRepository()
    {
        _stores = new List<Store>
        {
            new()
            {
                Id   = "S001",
                Name = "Apple Store",
                StoreAssignments = new List<StoreAssignment>
                {
                    new()
                    {
                        StoreId          = "S001",
                        AssignmentStatus = StoreAssignment.AssignmentStatusNew,
                        Status           = StoreAssignment.StatusInactive
                    },
                    new()
                    {
                        StoreId          = "S001",
                        AssignmentStatus = StoreAssignment.AssignmentStatusApproved,
                        Status           = StoreAssignment.StatusActive
                    }
                },
                Products = new List<Product>
                {
                    new() { StoreId = "S001", Name = "IPhone XR", Description = "Lorem ipsum" },
                    new() { StoreId = "S001", Name = "IPhone 11", Description = "Lorem ipsum" },
                    new() { StoreId = "S001", Name = "IPhone 12", Description = "Lorem ipsum" }
                },
                Locations = new List<Location>
                {
                    new() { StoreId = "S001", City = "Malang", Address   = "Jalan Malang" },
                    new() { StoreId = "S001", City = "Jakarta", Address  = "Jalan Jakarta" },
                    new() { StoreId = "S001", City = "Surabaya", Address = "Jalan Surabaya" }
                }
            },
            new()
            {
                Id   = "S002",
                Name = "Samsung Store",
                StoreAssignments = new List<StoreAssignment>
                {
                    new()
                    {
                        StoreId          = "S002",
                        AssignmentStatus = StoreAssignment.AssignmentStatusNew,
                        Status           = StoreAssignment.StatusInactive
                    },
                    new()
                    {
                        StoreId          = "S002",
                        AssignmentStatus = StoreAssignment.AssignmentStatusRejected,
                        Status           = StoreAssignment.StatusActive
                    }
                },
                Products = new List<Product>
                {
                    new() { StoreId = "S002", Name = "Samsung S21", Description = "Lorem ipsum" },
                    new() { StoreId = "S002", Name = "Samsung S22", Description = "Lorem ipsum" },
                    new() { StoreId = "S002", Name = "Samsung S23", Description = "Lorem ipsum" }
                },
                Locations = new List<Location>
                {
                    new() { StoreId = "S002", City = "Malang", Address   = "Jalan Malang" },
                    new() { StoreId = "S002", City = "Jakarta", Address  = "Jalan Jakarta" },
                    new() { StoreId = "S002", City = "Surabaya", Address = "Jalan Surabaya" }
                }
            },
            new()
            {
                Id   = "S003",
                Name = "Oppo Store",
                StoreAssignments = new List<StoreAssignment>
                {
                    new()
                    {
                        StoreId          = "S003",
                        AssignmentStatus = StoreAssignment.AssignmentStatusNew,
                        Status           = StoreAssignment.StatusInactive
                    },
                    new()
                    {
                        StoreId          = "S003",
                        AssignmentStatus = StoreAssignment.AssignmentStatusApproved,
                        Status           = StoreAssignment.StatusActive
                    }
                },
                Products = new List<Product>
                {
                    new() { StoreId = "S003", Name = "Oppo Reno 6", Description = "Lorem ipsum" },
                    new() { StoreId = "S003", Name = "Oppo Reno 7", Description = "Lorem ipsum" },
                    new() { StoreId = "S003", Name = "Oppo Reno 8", Description = "Lorem ipsum" }
                },
                Locations = new List<Location>
                {
                    new() { StoreId = "S003", City = "Malang", Address   = "Jalan Malang" },
                    new() { StoreId = "S003", City = "Jakarta", Address  = "Jalan Jakarta" },
                    new() { StoreId = "S003", City = "Surabaya", Address = "Jalan Surabaya" }
                }
            }
        };
    }

    public IQueryable<Store> GetAllStores()
        => _stores.AsQueryable()
            .Include(s => s.StoreAssignments)
            .Include(s => s.Products)
            .Include(s => s.Locations);
}