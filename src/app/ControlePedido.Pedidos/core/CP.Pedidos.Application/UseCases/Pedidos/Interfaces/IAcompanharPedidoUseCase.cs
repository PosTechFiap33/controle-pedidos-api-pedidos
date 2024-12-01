using CP.Pedidos.Application.DTOs;

namespace CP.Pedidos.Application.UseCases.Pedidos;

public interface IAcompanharPedidoUseCase
{
    Task<AcompanhamentoPedidoDTO> Executar(Guid pedidoId);
}
