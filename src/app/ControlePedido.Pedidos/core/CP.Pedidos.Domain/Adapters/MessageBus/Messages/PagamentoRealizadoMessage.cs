using System.Diagnostics.CodeAnalysis;

namespace CP.Pedidos.Domain.Adapters.MessageBus.Messages;

[ExcludeFromCodeCoverage]
public class PagamentoRealizadoMessage
{
    public Guid PagamentoId { get; set; }
    public Guid PedidoId { get; set; }
}
