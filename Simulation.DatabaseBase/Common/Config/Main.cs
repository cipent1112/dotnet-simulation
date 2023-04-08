using Microsoft.EntityFrameworkCore;
using Simulation.DatabaseBase.Common.Helpers;
using Simulation.DatabaseBase.Data.Contexts;

namespace Simulation.DatabaseBase.Common.Config;

public static class Main
{
    public static void AddAppService(this IServiceCollection service, IConfiguration conf)
        => service.AddAppDatabaseContext(conf);

    private static void AddAppDatabaseContext(this IServiceCollection service, IConfiguration conf)
    {
        service.AddDbContext<CoreDbContext>(opt => opt.UseSqlServer(conf.GenerateConnectionString("Core")));
        service.AddDbContext<LocationDbContext>(opt => opt.UseSqlServer(conf.GenerateConnectionString("Location")));
    }
}