using CP.Pedidos.Application.DTOs;

namespace CP.Pedidos.Application.UseCases.Pedidos;

public interface IPagarPedidoManualmenteUseCase
{
    Task Executar(Guid pedidoId, Guid pagamentoId);
}
