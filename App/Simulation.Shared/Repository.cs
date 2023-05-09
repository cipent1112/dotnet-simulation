using Microsoft.EntityFrameworkCore;
using Simulation.Shared.Models;

namespace Simulation.Shared;

public class Repository : IRepository
{
    private readonly DatabaseContext _db;

    public Repository(DatabaseContext db)
    {
        _db = db;
        var stores = new List<Store>
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
                    new()
                    {
                        Id          = "P001",
                        StoreId     = "S001",
                        Name        = "IPhone XR",
                        Description = "Lorem ipsum",
                        ProductImages = new List<ProductImage>
                            { new() { ProductId = "P001", Url = "iphonexr.com" } }
                    },
                    new()
                    {
                        Id          = "P002",
                        StoreId     = "S001",
                        Name        = "IPhone 11",
                        Description = "Lorem ipsum",
                        ProductImages = new List<ProductImage>
                            { new() { ProductId = "P002", Url = "iphone11.com" } }
                    },
                    new()
                    {
                        Id          = "P003",
                        StoreId     = "S001",
                        Name        = "IPhone 12",
                        Description = "Lorem ipsum",
                        ProductImages = new List<ProductImage>
                            { new() { ProductId = "P003", Url = "iphone12.com" } }
                    }
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
                    new()
                    {
                        Id          = "P004",
                        StoreId     = "S002",
                        Name        = "Samsung S21",
                        Description = "Lorem ipsum",
                        ProductImages = new List<ProductImage>
                            { new() { ProductId = "P004", Url = "samsungs21.com" } }
                    },
                    new()
                    {
                        Id          = "P005",
                        StoreId     = "S002",
                        Name        = "Samsung S22",
                        Description = "Lorem ipsum",
                        ProductImages = new List<ProductImage>
                            { new() { ProductId = "P005", Url = "samsungs22.com" } }
                    },
                    new()
                    {
                        Id          = "P006",
                        StoreId     = "S002",
                        Name        = "Samsung S23",
                        Description = "Lorem ipsum",
                        ProductImages = new List<ProductImage>
                            { new() { ProductId = "P006", Url = "samsungs23.com" } }
                    }
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
                    new()
                    {
                        Id          = "P007",
                        StoreId     = "S003",
                        Name        = "Oppo Reno 6",
                        Description = "Lorem ipsum",
                        ProductImages = new List<ProductImage>
                            { new() { ProductId = "P007", Url = "opporeno6.com" } }
                    },
                    new()
                    {
                        Id          = "P008",
                        StoreId     = "S003",
                        Name        = "Oppo Reno 7",
                        Description = "Lorem ipsum",
                        ProductImages = new List<ProductImage>
                            { new() { ProductId = "P008", Url = "opporeno7.com" } }
                    },
                    new()
                    {
                        Id          = "P009",
                        StoreId     = "S003",
                        Name        = "Oppo Reno 8",
                        Description = "Lorem ipsum",
                        ProductImages = new List<ProductImage>
                            { new() { ProductId = "P009", Url = "opporeno8.com" } }
                    }
                },
                Locations = new List<Location>
                {
                    new() { StoreId = "S003", City = "Malang", Address   = "Jalan Malang" },
                    new() { StoreId = "S003", City = "Jakarta", Address  = "Jalan Jakarta" },
                    new() { StoreId = "S003", City = "Surabaya", Address = "Jalan Surabaya" }
                }
            }
        };
        _db.Store.AddRange(stores);
        _db.SaveChanges();
    }

    public IQueryable<Store> Stores()
    {
        return _db.Store
            .Include(s => s.Products).ThenInclude(p => p.ProductImages)
            .Include(s => s.StoreAssignments)
            .Include(s => s.Locations).AsQueryable();
    }

    public IQueryable<Product> Products()
    {
        return _db.Product
            .Include(s => s.Store).ThenInclude(p => p.StoreAssignments)
            // .Include(s => s.Store).ThenInclude(p => p.Locations)
            .Include(s => s.ProductImages).AsQueryable();
    }
}