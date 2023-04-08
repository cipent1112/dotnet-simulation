using Simulation.DatabaseBase.Base.Extensions;
using Simulation.DatabaseBase.Common.Config;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("Main.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var service = builder.Services;
service.AddBaseService(configuration);
service.AddAppService(configuration);

var app = builder.Build();
app.AddComponentCors();
app.AddComponentForwardedHeaders();
app.UseRouting();
app.UseEndpoints(e => e.MapControllers());

await app.AppRunAsync();