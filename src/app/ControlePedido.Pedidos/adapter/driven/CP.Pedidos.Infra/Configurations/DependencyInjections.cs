using CP.Pedidos.CrpssCutting.Configuration;
using CP.Pedidos.Domain.Adapters.MessageBus;
using CP.Pedidos.Domain.Adapters.Providers;
using CP.Pedidos.Infra.Messaging;
using CP.Pedidos.Infra.Messaging.Workers;
using CP.Pedidos.Infra.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace CP.Pedidos.Infra.Configurations;

public static class RefitConfiguration
{
    /// <summary>
    /// Configura as dependências de infraestrutura para a aplicação.
    /// </summary>
    /// <param name="services">O contêiner de serviços do aplicativo.</param>
    /// <param name="configuration">A configuração do aplicativo.</param>
    /// <returns>O contêiner de serviços configurado.</returns>
    public static IServiceCollection AddInfraConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Configura as integrações
        services.Configure<AWSConfiguration>(configuration.GetSection("AWS"));

        services.Configure<Integrations>(configuration.GetSection("Integrations"));

        // Adiciona o serviço de mensagens e provedores
        services.AddScoped<IMessageBus, SqsMessageBus>();
        services.AddTransient<IPagamentoProvider, PagamentoProvider>();

        // Registra os clientes Refit dinamicamente
        services.AddRefitClientsFromConfiguration();

        services.AddHostedService<PagamentoMessagingWorker>();

        return services;
    }

    /// <summary>
    /// Registra dinamicamente clientes Refit com base nas configurações de integrações.
    /// </summary>
    /// <param name="services">O contêiner de serviços do aplicativo.</param>
    private static void AddRefitClientsFromConfiguration(this IServiceCollection services)
    {
        // Recupera a configuração de integrações
        var serviceProvider = services.BuildServiceProvider();
        var integrationConfig = serviceProvider.GetRequiredService<IOptions<Integrations>>().Value;

        if (integrationConfig.ApiConfigurations == null || !integrationConfig.ApiConfigurations.Any())
            throw new InvalidOperationException("Nenhuma configuração de API encontrada em 'Integrations'.");
        

        foreach (var api in integrationConfig.ApiConfigurations)
        {
            try
            {
                // Obtém o tipo da interface a partir do nome
                var apiInterface = Type.GetType($"CP.Pedidos.Infra.Communications.{api.ServiceName}");

                if (apiInterface == null)
                    throw new InvalidOperationException($"Interface '{api.ServiceName}' não encontrada.");

                // Registra o cliente Refit
                services.AddRefitClient(apiInterface)
                        .ConfigureHttpClient(c => c.BaseAddress = new Uri(api.Url));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao registrar o cliente Refit para '{api.ServiceName}': {ex.Message}", ex);
            }
        }
    }
}
