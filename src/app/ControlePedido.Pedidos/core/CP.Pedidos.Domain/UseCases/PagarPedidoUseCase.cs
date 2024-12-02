namespace CP.Pedidos.Domain.UseCases;

public interface IPagarPedidoUseCase
{
    Task Executar(Guid pedidoId, Guid pagamentoId);
}