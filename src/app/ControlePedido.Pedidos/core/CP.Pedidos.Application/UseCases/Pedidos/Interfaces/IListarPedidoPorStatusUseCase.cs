using CP.Pedidos.Application.DTOs;
using CP.Pedidos.Domain.Enums;

namespace CP.Pedidos.Application.UseCases.Pedidos
{
    public interface IListarPedidoUseCase
    {
        Task<ICollection<PedidoDTO>> Executar(StatusPedido? status);
    }
}

