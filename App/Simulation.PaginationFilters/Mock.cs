using Simulation.PaginationFilters.Models;

namespace Simulation.PaginationFilters;

public static class Mock
{
    public static IEnumerable<Store> Stores => new List<Store>
    {
        new()
        {
            Id = "1",
            Name = "Apple Inc", 
            Code = "A001", 
            PhoneNumber = "0812", 
            Address = "California", 
            Status = Store.StatusActive,
            CreatedAt = DateTime.Parse("2023-04-01 06:00:00"),
            Products = new List<Product>
            {
                new()
                {
                    Id = "1",
                    StoreId = "1",
                    Name = "IPhone XR",
                    Code = "AP001",
                    Price = 399.99,
                    Stock = 5,
                    Status = Product.StatusAvailable,
                    CreatedAt = DateTime.Parse("2023-04-01 06:30:00")
                },
                new()
                {
                    Id = "2",
                    StoreId = "1",
                    Name = "IPhone 11",
                    Code = "APP002",
                    Price = 499,
                    Stock = 5,
                    Status = Product.StatusAvailable,
                    CreatedAt = DateTime.Parse("2023-04-01 06:30:00")
                }
            }
        },
        new()
        {
            Id = "2",
            Name = "Samsung", 
            Code = "A002", 
            PhoneNumber = "0813", 
            Address = "South Korea", 
            Status = Store.StatusActive,
            CreatedAt = DateTime.Parse("2023-04-02 15:00:00"),
            Products = new List<Product>
            {
                new()
                {
                    Id = "3",
                    StoreId = "2",
                    Name = "Samsung S20",
                    Code = "SP001",
                    Price = 750,
                    Stock = 21,
                    Status = Product.StatusAvailable,
                    CreatedAt = DateTime.Parse("2023-04-02 15:30:00")
                },
                new()
                {
                    Id = "4",
                    StoreId = "2",
                    Name = "Samsung S21",
                    Code = "SP002",
                    Price = 999,
                    Stock = 10,
                    Status = Product.StatusAvailable,
                    CreatedAt = DateTime.Parse("2023-04-02 18:30:00")
                }
            }
        }
    }.AsQueryable();
}