using System.Linq.Dynamic.Core;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simulation.BaseAction.Actions;
using Simulation.BaseAction.Constants;
using Simulation.BaseAction.Filters;
using Simulation.Shared;
using Simulation.Shared.Models;

namespace Simulation.BaseAction;

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
                    var (count, result) = BuildProvinces(repo, filters);

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

    private static List<QueryParam> BuildFilters(string filtersPayloadBase64)
    {
        var filtersString = Encoding.UTF8.GetString(Convert.FromBase64String(filtersPayloadBase64));
        var filtersObj    = JsonConvert.DeserializeObject<Dictionary<string, object>>(filtersString);

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine();
        Console.WriteLine("Filters Json: {0}", JsonConvert.SerializeObject(filtersObj, Formatting.Indented));
        Console.WriteLine();

        return (from filter in filtersObj!
            let values = ((JArray)filter.Value).ToObject<List<object>>()
            select new QueryParam
            {
                Field    = filter.Key,
                Operator = values![0].ToString()!,
                Value    = values[1]
            }).ToList();
    }

    private static (int count, object) BuildProvinces(IRepository repo, List<QueryParam> queryParams)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Start to load province list...");
        Console.ResetColor();

        var allowedFilter = new List<AllowedFilter>
        {
            new()
            {
                Relations = new List<string>
                    { nameof(Province.Regencies), nameof(Regency.Districts), nameof(District.Villages) },
                Filters = new List<Filter>
                {
                    new() { Property = nameof(Village.Name) },
                    new()
                    {
                        Property = nameof(Village.Status), FilterOperand = Operand.LikeOperator, FilterValue = "Active"
                    },
                    new() { Property = nameof(Village.PostalCode), FilterKey = "VillagePostalCode" }
                }
            },
            new()
            {
                Filters = new List<Filter>
                {
                    new() { Property = nameof(Province.Name), FilterKey = "ProvinceName" }
                }
            },
            new()
            {
                Relations = new List<string> { nameof(Province.Regencies), nameof(Regency.Districts) },
                Filters = new List<Filter>
                {
                    new() { Property = nameof(District.Name), FilterKey = "DistrictName" }
                }
            }
        };

        var provinces = new ListAction(allowedFilter).ApplyFilter(repo.Provinces(), queryParams);

        /* Manual (Testing) */
        // var provinces = repo.Provinces();
        // provinces = provinces.Where(
        //     "Regencies.Any(Districts.Any(Villages.Any(  Status.Contains(@0) && PostalCode = @1)))",
        //     "Active", "asd"
        // );

        return (provinces.Count(), provinces.Select(p => new
        {
            p.Id,
            p.Name,
            RegionName = p.RegionProvinces
                .Select(rp => new { RegionName = rp.Region.Name })
                .FirstOrDefault()
        }).ToList());
    }
}