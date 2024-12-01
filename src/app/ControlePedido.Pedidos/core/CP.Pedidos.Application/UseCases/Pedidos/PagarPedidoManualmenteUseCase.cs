using CP.Pedidos.Application.UseCases.Pedidos;
using CP.Pedidos.Domain.Adapters.Repositories;
using CP.Pedidos.Domain.Base;

namespace ControlePedido.Application.UseCases.Pedidos;

public class PagarPedidoManualmenteUseCase : IPagarPedidoManualmenteUseCase
{
    private readonly IPedidoRepository _repository;

    public PagarPedidoManualmenteUseCase(IPedidoRepository repository)
    {
        _repository = repository;
    }

    public async Task Executar(Guid pedidoId, Guid pagamentoId)
    {
        if(Guid.Empty == pedidoId)
            throw new DomainException("O código do pedido não foi informado!");

        var pedido = await _repository.ConsultarPorId(pedidoId);

        if (pedido is null)
            throw new DomainException("Não foi encontrado um pedido com o código informado!");

        pedido.Pagar(pagamentoId);

        _repository.Atualizar(pedido);

        await _repository.UnitOfWork.Commit();
    }
}
