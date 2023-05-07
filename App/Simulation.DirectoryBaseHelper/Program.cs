using Simulation.DirectoryBaseHelper.Extensions;

var appBuilder = ConfigureBuilderExtension.CreateWebApplicationBuilder(args);
appBuilder.AddConfigureWebHostBuilder();
appBuilder.AddGeneralServices();

var app = appBuilder.Build();
app.AddGeneralPipelines();
await app.RunAsync();