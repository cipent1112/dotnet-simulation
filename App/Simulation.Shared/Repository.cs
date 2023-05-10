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

        var provinces = new List<Province>
        {
            new()
            {
                Id = "p1", Name = "Jawa Timur", Status = "Active",
                ProvinceAssignments = new List<ProvinceAssignment>
                {
                    new() { Id = "pa1", ProvinceId = "p1", AssignmentStatus = "Approved", Status = "Active" }
                },
                Regencies = new List<Regency>
                {
                    new()
                    {
                        Id = "r1", ProvinceId = "p1", Code = "malang", Name = "Kota Malang", Status = "Active",
                        Districts = new List<District>
                        {
                            new()
                            {
                                Id = "d1", RegencyId = "r1", Name = "Lowokwaru", Status = "Active",
                                Villages = new List<Village>
                                {
                                    new()
                                    {
                                        Id     = "v1", DistrictId = "d1", Name = "Tunjungsekar", PostalCode = "001",
                                        Status = "Active"
                                    },
                                    new()
                                    {
                                        Id     = "v2", DistrictId = "d1", Name = "Mojolangu", PostalCode = "002",
                                        Status = "Active"
                                    }
                                }
                            }
                        }
                    },
                    new()
                    {
                        Id = "r2", ProvinceId = "p1", Code = "surabaya", Name = "Kota Surabaya", Status = "Active",
                        Districts = new List<District>
                        {
                            new()
                            {
                                Id = "d2", RegencyId = "r2", Name = "Kenjeran", Status = "Active",
                                Villages = new List<Village>
                                {
                                    new()
                                    {
                                        Id     = "v3", DistrictId = "d2", Name = "Bulakbanteng", PostalCode = "003",
                                        Status = "Active"
                                    },
                                    new()
                                    {
                                        Id     = "v4", DistrictId = "d2", Name = "Tambakwedi", PostalCode = "004",
                                        Status = "Active"
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        _db.Province.AddRange(provinces);
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
            .Include(s => s.ProductImages).AsQueryable();
    }

    public IQueryable<Province> Provinces()
    {
        return _db.Province
            .Include(_ => _.Regencies)
            .ThenInclude(_ => _.Districts)
            .ThenInclude(_ => _.Villages)
            .AsQueryable();
    }

    public IQueryable<Regency> Regencies()
    {
        return _db.Regency
            .Include(_ => _.Province).ThenInclude(_ => _.ProvinceAssignments)
            .Include(_ => _.Districts)
            .ThenInclude(_ => _.Villages)
            .AsQueryable();
    }
}