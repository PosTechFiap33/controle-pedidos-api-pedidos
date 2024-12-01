using CP.Pedidos.Application.DTOs;

namespace CP.Pedidos.Application.UseCases.Pedidos
{
    public interface ICriarPedidoUseCase
    {
        Task<PedidoCriadoDTO> Executar(CriarPedidoDTO criarPedidoDTO);
    }
}

