using CP.Pedidos.Domain.UseCases;
using CP.Pedidos.CrpssCutting.Configuration;
using CP.Pedidos.Domain.Adapters.MessageBus;
using CP.Pedidos.Domain.Adapters.MessageBus.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace CP.Pedidos.Infra.Messaging.Workers;

public class PagamentoMessagingWorker : MessagingWorker<PagamentoRealizadoMessage>
{
    public PagamentoMessagingWorker(ILogger<PagamentoMessagingWorker> logger,
                                    IServiceProvider serviceProvider,
                                    IOptions<AWSConfiguration> options)
                                    : base(logger, serviceProvider, options.Value.PagamentoQueueUrl)
    {
    }

    protected override async Task ProccessMessage(PagamentoRealizadoMessage message, IServiceScope scope)
    {
        var useCase = scope.ServiceProvider.GetService<IPagarPedidoUseCase>();
        await useCase.Executar(message.PedidoId, message.PagamentoId);
    }
}
