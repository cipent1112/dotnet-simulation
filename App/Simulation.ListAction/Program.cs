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

    private static (int count, object) BuildDistricts(IRepository repo, List<PropertiesFilter> filters)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Start to load district list...");
        Console.ResetColor();

        var districts = repo.Districts();
        var allowedFilterProperty = new List<AllowedFilterProperty>
        {
            new()
            {
                Key                = "RegencyName",
                RelationProperties = new[] { nameof(Regency) },
                FilterProperty     = nameof(Regency.Name)
            }
        };

        districts = new ListAction(allowedFilterProperty).ApplyFilter(districts, filters);
        return (districts.Count(), districts.Select(s => new
        {
            s.Id,
            s.Name,
            RegencyName  = s.Regency.Name,
            ProvinceName = s.Regency.Province.Name,
            RegionProvince = s.Regency.Province.RegionProvinces.Select(_ => new
            {
                RegionName   = _.Region.Name,
                ProvinceName = _.Province.Name,
                _.RegionId,
                _.ProvinceId
            })
        }).ToList());
    }

    private static (int count, object) BuildRegencies(IRepository repo, List<PropertiesFilter> filters)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Start to load regency list...");
        Console.ResetColor();

        var regencies = repo.Regencies();
        var allowedFilterProperty = new List<AllowedFilterProperty>
        {
            new()
            {
                Key = "VillageName",
                RelationProperties = new[]
                {
                    nameof(Province),
                    nameof(Province.RegionProvinces),
                    nameof(RegionProvince.Region)
                },
                FilterProperty = nameof(Region.Name)
            }
        };

        regencies = new ListAction(allowedFilterProperty).ApplyFilter(regencies, filters);
        return (regencies.Count(), regencies.Select(r => new
        {
            r.Id,
            r.Name,
            ProvinceName = r.Province.Name,
            RegionProvinces = r.Province.RegionProvinces
                .Select(rp => new { RegionName = rp.Region.Name })
                .FirstOrDefault()
        }).ToList());
    }

    private static (int count, object) BuildProvinces(IRepository repo, List<PropertiesFilter> filters)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Start to load province list...");
        Console.ResetColor();

        var provinces = repo.Provinces();
        var allowedFilterProperty = new List<AllowedFilterProperty>
        {
            new()
            {
                Key = "RegionName",
                RelationProperties = new[]
                {
                    nameof(Province.RegionProvinces),
                    nameof(Region)
                },
                FilterProperty = nameof(Region.Name)
            }
        };

        provinces = new ListAction(allowedFilterProperty).ApplyFilter(provinces, filters);
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