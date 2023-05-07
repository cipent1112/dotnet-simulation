using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simulation.ListAction;
using Simulation.Shared;

while (true)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;

    Console.WriteLine("Paste your base64 filters here: ");
    var filtersPayloadBase64 = Console.ReadLine();

    try
    {
        var stores = MockRepository.Stores;

        var filtersString = Encoding.UTF8.GetString(Convert.FromBase64String(filtersPayloadBase64!));
        var filtersObj    = JsonConvert.DeserializeObject<Dictionary<string, object>>(filtersString);

        var filterList = (from filter in filtersObj!
            let values = ((JArray)filter.Value).ToObject<List<object>>()
            select new PropertiesFilter
            {
                Field    = filter.Key,
                Operator = values![0].ToString()!,
                Value    = values[1]
            }).ToList();

        var allowedPropertyFilters = new List<AllowedPropertyFilter>
        {
            new()
            {
                ParamKey     = "StoreName",
                PropertyName = nameof(Store.Name)
            },
            new()
            {
                ParamKey     = "CurrentAssignment",
                RelationName = nameof(Store.StoreAssignments),
                PropertyName = nameof(StoreAssignment.AssignmentStatus)
            }
        };

        stores = new PagedAction(allowedPropertyFilters).ApplyFilters(stores, filterList);
        var result = stores
            .Select(s => new
            {
                s.Id,
                s.Name,
                AssignmentStatus = s.CurrentAssignment != null ? s.CurrentAssignment.AssignmentStatus : null
            })
            .ToList();

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine();
        Console.WriteLine("Filters Json: {0}", JsonConvert.SerializeObject(filtersObj, Formatting.Indented));
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Total record found: {0}", stores.Count());
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
        Console.WriteLine("Error message: {0}", e.StackTrace);
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