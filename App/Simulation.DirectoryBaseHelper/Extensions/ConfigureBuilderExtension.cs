using Simulation.DirectoryBaseHelper.Common.Helper;

namespace Simulation.DirectoryBaseHelper.Extensions;

public static class ConfigureBuilderExtension
{
    private static string[] Args { get; set; } = null!;
    private static IConfigurationRoot ConfigurationRoot { get; set; } = null!;
    public static AppSetting AppSetting { get; set; } = null!;

    public static WebApplicationBuilder CreateWebApplicationBuilder(string[] args)
    {
        InitializeConfigureBuilder(args);
        return WebApplication.CreateBuilder(GetConfigurationWebApplicationOptions());
    }

    public static void AddConfigureWebHostBuilder(this WebApplicationBuilder appBuilder)
    {
        appBuilder.WebHost
            .UseUrls(AppSetting.AppUrl.Split(';'))
            .UseKestrel()
            .ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 10_485_760);
    }

    private static void InitializeConfigureBuilder(string[] args)
    {
        Args = args;
        var configurationBuilder = new ConfigurationBuilder();
        ConfigurationRoot = configurationBuilder.AddConfigurationRoot();
        AppSetting = ConfigurationRoot.GetAppSettings();
    }

    private static IConfigurationRoot AddConfigurationRoot(this IConfigurationBuilder configurationBuilder)
        => configurationBuilder
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Common", "Config"))
            .AddJsonFile("Main.json")
            .AddJsonFile("MainLocal.json")
            .AddEnvironmentVariables()
            .Build();

    private static AppSetting GetAppSettings(this IConfiguration configurationRoot)
        => configurationRoot.GetSection(nameof(AppSetting)).Get<AppSetting>();

    private static WebApplicationOptions GetConfigurationWebApplicationOptions() => new()
    {
        ApplicationName = typeof(Program).Assembly.FullName,
        ContentRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
        WebRootPath = "wwwroot",
        Args = Args,
        EnvironmentName = AppSetting.AppEnv ?? Environments.Development
    };
}