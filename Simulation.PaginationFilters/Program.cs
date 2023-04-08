using System.Linq.Dynamic.Core;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simulation.PaginationFilters;
using Simulation.PaginationFilters.Models;

while (true)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;

    Console.WriteLine("Paste your base64 filters here: ");
    var filtersPayloadBase64 = Console.ReadLine();

    var stores = Mock.Stores.AsQueryable();

    try
    {
        var entityProperties = typeof(Store).GetProperties();
        var filtersString = Encoding.UTF8.GetString(Convert.FromBase64String(filtersPayloadBase64!));
        var filtersObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(filtersString);

        var filterList = (from filter in filtersObj!
            let values = ((JArray)filter.Value).ToObject<List<object>>()
            select new FilterProperties
            {
                Field = filter.Key,
                Operator = values![0].ToString()!,
                Value = values[1]
            }).ToList();

        foreach (var filter in filterList)
        {
            var prop = entityProperties.FirstOrDefault(e => e.Name.Equals(filter.Field));

            if (prop != null)
            {
                switch (filter.Operator)
                {
                    case FilterOperators.EqualOperator:
                        stores = stores.Where($"{filter.Field} == @0", filter.Value);
                        break;
                    case FilterOperators.NotEqualOperator:
                        stores = stores.Where($"{filter.Field} != @0", filter.Value);
                        break;
                    case FilterOperators.LikeOperator:
                        stores = stores.Where($"{filter.Field}.Contains(@0)", filter.Value);
                        break;
                    case FilterOperators.NotLikeOperator:
                        stores = stores.Where($"!{filter.Field}.Contains(@0)", filter.Value);
                        break;
                    case FilterOperators.BetweenOperator:
                    {
                        object from = null!;
                        object until = null!;
                        var allowBetween = new[]
                            {
                                typeof(DateTime),
                                typeof(double),
                                typeof(int),
                                typeof(long),
                                typeof(float),
                                typeof(decimal)
                            }
                            .Contains(prop.PropertyType);

                        if (allowBetween)
                        {
                            if (prop.PropertyType == typeof(DateTime))
                            {
                                from = DateTime.Parse(((JArray)filter.Value)[0].ToString());
                                until = DateTime.Parse(((JArray)filter.Value)[1].ToString());
                            }
                            else
                            {
                                from = Convert.ToDouble(((JArray)filter.Value)[0]);
                                until = Convert.ToDouble(((JArray)filter.Value)[1]);
                            }
                        }

                        stores = stores.Where($"{filter.Field} >= @0 AND {filter.Field} <= @1", from, until);
                        break;
                    }
                    case FilterOperators.LessThanOperator:
                    {
                        stores = stores.Where($"{filter.Field} < @0", filter.Value);
                        break;
                    }
                    case FilterOperators.LessThanEqualOperator:
                    {
                        stores = stores.Where($"{filter.Field} <= @0", filter.Value);
                        break;
                    }
                    case FilterOperators.GreaterThanOperator:
                    {
                        stores = stores.Where($"{filter.Field} > @0", filter.Value);
                        break;
                    }
                    case FilterOperators.GreaterThanEqualOperator:
                    {
                        stores = stores.Where($"{filter.Field} >= @0", filter.Value);
                        break;
                    }
                    case FilterOperators.InOperator:
                    {
                        stores = stores.Where($"{(JArray)filter.Value}.Contains(@0)", filter.Field);
                        break;
                    }
                    case FilterOperators.NotInOperator:
                    {
                        stores = stores.Where($"!{(JArray)filter.Value}.Contains(@0)", filter.Field);
                        break;
                    }
                    default: continue;
                }
            }

            var data = stores
                .Include(s => s.Products)
                .Select(s => new
                {
                    StoreId = s.Id,
                    StoreName = s.Name,
                    StoreAddress = s.Address,
                    Products = s.Products.Select(p => new
                    {
                        ProductId = p.Id,
                        ProductName = p.Name,
                        ProductPrice = p.Price.ToString("C0"),
                        ProductStock = p.Stock
                    }),
                    CreatedAtFormatted = s.CreatedAt.ToString("dd MMMM yyyy HH:mm:ss")
                }).AsEnumerable();

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine();
            Console.WriteLine("Filters Json: {0}", JsonConvert.SerializeObject(filtersObj, Formatting.Indented));
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Total record found: {0}", stores.Count());
            Console.WriteLine("Results: ");
            Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write("Try again? (y/n) ");
            var tryAgain = Console.ReadLine();

            if (!string.IsNullOrEmpty(tryAgain) && tryAgain.ToLower().Equals("y")) continue;
            break;
        }
    }
    catch (Exception e)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("Error message: {0}", e.Message);
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write("Press anything to retry...");
        Console.ReadLine();
    }
    finally
    {
        Console.ResetColor();
    }
}