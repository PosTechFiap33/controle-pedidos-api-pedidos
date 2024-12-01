using CP.Pedidos.Application.Configurations;
using CP.Pedidos.Data.Configuration;
using CP.Pedidos.Infra.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CP.Pedidos.IOC.DependencyInjections;

public static class ApplicationDependencyInjections
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfraConfiguration(configuration);
        services.AddApplicationConfiguration();
        services.AddDatabaseConfiguration(configuration);
    }
}
