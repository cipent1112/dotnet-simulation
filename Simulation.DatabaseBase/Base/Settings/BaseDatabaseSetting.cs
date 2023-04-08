namespace Simulation.DatabaseBase.Base.Settings;

public class BaseDatabaseSetting
{
    public string Driver { get; set; }
    public bool AutoMigrate { get; set; }
    public string? CollationType { get; set; }
    public string InstanceName { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public bool IntegratedSecurity { get; set; } = true;
    public bool TrustServerCertificate { get; set; }
    public bool MultipleActiveResultSets { get; set; } = true;
    public string? Username { get; set; }
    public string? Password { get; set; }
}