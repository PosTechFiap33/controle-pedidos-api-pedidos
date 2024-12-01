using CP.Pedidos.Application.DTOs;
using CP.Pedidos.Domain.Adapters.Repositories;
using CP.Pedidos.Domain.Base;

namespace CP.Pedidos.Application.UseCases.Pedidos;

public class AcompanharPedidoUseCase : IAcompanharPedidoUseCase
{
    private readonly IPedidoRepository _repository;

    public AcompanharPedidoUseCase(IPedidoRepository repository)
    {
        _repository = repository;
    }

    public async Task<AcompanhamentoPedidoDTO> Executar(Guid pedidoId)
    {
        var pedido = await _repository.ConsultarPorId(pedidoId);

        if(pedido is null)
            throw new DomainException("Pedido não encontrado para o código informado");

        return new AcompanhamentoPedidoDTO(pedido);
    }
}
