using Simulation.DatabaseBase.Base.Settings;

namespace Simulation.DatabaseBase.Common.Helpers;

public static class DatabaseHelper
{
    public static string GenerateConnectionString(this IConfiguration conf, string sectionName)
    {
        try
        {
            var section = conf.GetSection("DatabaseSettings").GetSection(sectionName);
            if (section == null) throw new Exception("Invalid database setting section.");
            var dbSetting = section.Get<BaseDatabaseSetting>();
            if (dbSetting == null) throw new Exception("Invalid database setting.");
            
            return $"Data Source={dbSetting.InstanceName};" +
                   $"Initial Catalog={dbSetting.DatabaseName};" +
                   $"Integrated Security={dbSetting.IntegratedSecurity.ToString()};" +
                   $"TrustServerCertificate={dbSetting.TrustServerCertificate.ToString()};" +
                   $"MultipleActiveResultSets={dbSetting.MultipleActiveResultSets.ToString()};" +
                   $"User id={dbSetting.Username};" +
                   $"Password={dbSetting.Password}";
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}