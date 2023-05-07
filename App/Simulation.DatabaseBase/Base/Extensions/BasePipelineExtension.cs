using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Simulation.DatabaseBase.Base.Settings;

namespace Simulation.DatabaseBase.Base.Extensions;

public static class BasePipelineExtension
{
    public static void AddComponentCors(this IApplicationBuilder ab) =>
        ab.UseCors(cpb => cpb
            .SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
    
    public static void AddComponentForwardedHeaders(this IApplicationBuilder ab) =>
        ab.UseForwardedHeaders(new ForwardedHeadersOptions
            { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
    
    public static async Task AppRunAsync(this WebApplication wa)
    {
        var appSetting = wa.Services.GetRequiredService<IOptions<AppSetting>>().Value;
        var ub = new UriBuilder(appSetting.AppUrl);

        wa.MapGet("/", (HttpContext context) => JsonConvert.SerializeObject(new
        {
            Name = appSetting.AppName,
            Message = $"{appSetting.AppName} is running",
            Data = context.Connection.RemoteIpAddress?.MapToIPv4().ToString()
        }));

        wa.Environment.EnvironmentName = appSetting.AppEnv;
        await wa.RunAsync(ub.Uri.AbsoluteUri);
    }
}