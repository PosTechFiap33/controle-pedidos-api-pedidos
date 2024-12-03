using System.Diagnostics.CodeAnalysis;
using CP.Pedidos.Domain.Adapters.MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CP.Pedidos.Infra.Messaging.Workers;

[ExcludeFromCodeCoverage]
public abstract class MessagingWorker<T> : BackgroundService
{
    protected readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessagingWorker<T>> _logger;
    private readonly string _queueUrl;
    protected abstract Task ProccessMessage(T message, IServiceScope serviceScope);


    public MessagingWorker(ILogger<MessagingWorker<T>> logger,
                           IServiceProvider serviceProvider,
                           string queueUrl)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _queueUrl = queueUrl;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando observação da fila SQS...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

                    // var messageBus = GetService<IMessageBus>();
                    var messages = await messageBus.ReceiveMessagesAsync<T>(_queueUrl);

                    foreach (var message in messages)
                    {
                        await ProccessMessage(message.data, scope);

                        _logger.LogInformation($"Mensagem recebida: {message}");

                        await messageBus.DeleteMessage(_queueUrl, message.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagens da fila.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    // protected TService GetService<TService>()
    // {
    //     using var scope = _serviceProvider.CreateScope();
    //     return scope.ServiceProvider.GetRequiredService<TService>();
    // }
}