namespace Simulation.DirectoryBaseHelper.Extensions;

public static class ConfigureServiceExtension
{
    public static void AddGeneralServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddControllers();
    }
}