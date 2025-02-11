using Microsoft.Extensions.DependencyInjection;
using ZooERP.Services;

namespace ZooERP.Configuration;

public static class DependencyInjectionConfig
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<VeterinaryClinic>();
        services.AddSingleton<Zoo>();

        return services;
    }
}
