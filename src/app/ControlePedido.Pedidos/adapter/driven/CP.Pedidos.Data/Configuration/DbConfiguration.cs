using CP.Pedidos.Domain.Adapters.Repositories;
using ControlePedido.Infra;
using ControlePedido.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CP.Pedidos.Data.Configuration;

public static class DynamoDbConfiguration
{

    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration){
        var connectionEnv = "DbConnection";
        var connectionString = Environment.GetEnvironmentVariable(connectionEnv) ?? configuration[connectionEnv];
        services.AddDbContext<ControlePedidoContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<ControlePedidoContext>();
        services.AddTransient<IPedidoRepository, PedidoRepository>();
            
        return services;
    }

    public static void ConfigureMigrationDatabase(this IServiceProvider services)
    {
        try
        {
            var dbContext = services.GetRequiredService<ControlePedidoContext>();
            dbContext.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<ControlePedidoContext>>();
            logger.LogError(ex, "Ocorreu um erro ao executar a migration do banco de dados!");
        }
    }
}
