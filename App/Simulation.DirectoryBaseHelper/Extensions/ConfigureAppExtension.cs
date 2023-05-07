using Microsoft.AspNetCore.HttpOverrides;
using Simulation.DirectoryBaseHelper.Common.Helper;

namespace Simulation.DirectoryBaseHelper.Extensions;

public static class ConfigureAppExtension
{
    public static void AddGeneralPipelines(this IApplicationBuilder app)
    {
        app.ApplicationServices.DirectoryHelperConfigure();
        app.UseRouting()
            .UseEndpoints(endpoints => endpoints.MapControllers())
            .UseForwardedHeaders(new ForwardedHeadersOptions
                { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto })
            .UseCors(builder => builder
                .SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );
    }
}