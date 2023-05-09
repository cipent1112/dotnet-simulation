using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simulation.Shared;
using Simulation.Shared.Models;

namespace Simulation.ListAction;

internal static class Program
{
    private static Task Main()
    {
        using var host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase("SharedDatabase"));
                services.AddScoped<IRepository, Repository>();
            })
            .Build();

        using var scope = host.Services.CreateScope();
        {
            var repo = scope.ServiceProvider.GetRequiredService<IRepository>();
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Paste your base64 filters here: ");
                var filtersPayloadBase64 = Console.ReadLine();

                try
                {
                    var filters = BuildFilters(filtersPayloadBase64!);
                    var (count, result) = BuildProductList(repo, filters);

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine();
                    Console.WriteLine("Total record found: {0}", count);
                    Console.WriteLine("Results: ");
                    Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.Write("Try again? (y/n) ");
                    var tryAgain = Console.ReadLine();

                    if (!string.IsNullOrEmpty(tryAgain) && tryAgain.ToLower().Equals("y")) continue;
                    break;
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Error message: {0}", e.Message);
                    Console.WriteLine("Stack Trace: {0}", e.StackTrace);
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
        }
        return Task.CompletedTask;
    }

    private static List<PropertiesFilter> BuildFilters(string filtersPayloadBase64)
    {
        var filtersString = Encoding.UTF8.GetString(Convert.FromBase64String(filtersPayloadBase64));
        var filtersObj    = JsonConvert.DeserializeObject<Dictionary<string, object>>(filtersString);

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine();
        Console.WriteLine("Filters Json: {0}", JsonConvert.SerializeObject(filtersObj, Formatting.Indented));
        Console.WriteLine();

        return (from filter in filtersObj!
            let values = ((JArray)filter.Value).ToObject<List<object>>()
            select new PropertiesFilter
            {
                Field    = filter.Key,
                Operator = values![0].ToString()!,
                Value    = values[1]
            }).ToList();
    }

    private static (int count, object) BuildStoreList(IRepository repo, List<PropertiesFilter> filters)
    {
        var stores = repo.Stores();
        var allowedPropertyFilters = new List<AllowedPropertyFilter>
        {
            new()
            {
                ParamKey       = "StoreName",
                FilterProperty = nameof(Store.Name)
            },
            new()
            {
                ParamKey         = "CurrentAssignment",
                RelationProperty = nameof(Store.StoreAssignments),
                FilterProperty   = nameof(StoreAssignment.AssignmentStatus)
            }
        };

        stores = new PagedAction(allowedPropertyFilters).ApplyFilter(stores, filters);
        return (stores.Count(), stores.Select(s => new
        {
            s.Id,
            s.Name,
            AssignmentStatus = s.CurrentAssignment != null
                ? s.CurrentAssignment.AssignmentStatus
                : null
        }).ToList());
    }

    private static (int count, object) BuildProductList(IRepository repo, List<PropertiesFilter> filters)
    {
        var products = repo.Products();
        var allowedPropertyFilters = new List<AllowedPropertyFilter>
        {
            new()
            {
                ParamKey = "Name"
            },
            new()
            {
                ParamKey       = "ProductName",
                FilterProperty = nameof(Product.Name)
            },
            new()
            {
                ParamKey         = "StoreName",
                RelationProperty = nameof(Product.Store),
                FilterClass      = typeof(Store),
                FilterProperty   = nameof(Store.Name)
            },
            new()
            {
                ParamKey         = "CurrentAssignment",
                RelationClass    = typeof(Store),
                RelationProperty = nameof(Store.StoreAssignments),
                FilterClass      = typeof(StoreAssignment),
                FilterProperty   = nameof(StoreAssignment.AssignmentStatus)
            }
        };

        products = new PagedAction(allowedPropertyFilters).ApplyFilter(products, filters);
        return (products.Count(), products.Select(p => new
        {
            p.Id,
            p.Name,
            p.Description,
            Store = new
            {
                p.Store.Id,
                p.Store.Name,
                CurrentAssignment = p.Store.CurrentAssignment != null
                    ? p.Store.CurrentAssignment.AssignmentStatus
                    : null
            }
        }).ToList());
    }
}