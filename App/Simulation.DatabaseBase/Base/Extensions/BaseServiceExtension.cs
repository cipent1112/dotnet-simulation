﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Simulation.DatabaseBase.Base.Database;
using Simulation.DatabaseBase.Base.Settings;

namespace Simulation.DatabaseBase.Base.Extensions;

public static class BaseServiceExtension
{
    public static void AddBaseService(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddBaseController();
        service.AddBaseSettings(configuration);
        service.AddConfigureApiBehaviorOptions();
        BaseBehavior.SetIdentityOwnerId(identityOwnerId: "tes");
    }
    
    private static void AddBaseSettings(this IServiceCollection service, IConfiguration configuration)
        => service.Configure<AppSetting>(configuration.GetSection("AppSetting"));

    private static void AddBaseController(this IServiceCollection service)
    {
        service.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            options.SerializerSettings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new DefaultNamingStrategy()
            };
        });
        service.AddRouting(options => options.LowercaseUrls = true);
    }

    private static void AddConfigureApiBehaviorOptions(this IServiceCollection sc) =>
        sc.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
}