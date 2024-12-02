namespace CP.Pedidos.Domain.Adapters.MessageBus.Messages;

public class PagamentoRealizadoMessage
{
    public Guid PagamentoId { get; set; }
    public Guid PedidoId { get; set; }
}
