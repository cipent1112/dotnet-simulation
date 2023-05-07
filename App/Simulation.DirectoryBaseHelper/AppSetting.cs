namespace Simulation.DirectoryBaseHelper;

public class AppSetting
{
    public string AppName { get; set; } = null!;
    public string AppUrl { get; set; } = null!;
    public string? AppEnv { get; set; } = Environments.Development;
}