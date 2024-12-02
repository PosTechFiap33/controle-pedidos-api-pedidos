using CP.Pedidos.Domain.Adapters.Repositories;
using CP.Pedidos.Domain.UseCases;

namespace CP.Pedidos.Application.UseCases.Pedidos;

public class PagarPedidoUseCase : IPagarPedidoUseCase
{
    private readonly IPedidoRepository _repository;

    public PagarPedidoUseCase(IPedidoRepository repository)
    {
        _repository = repository;
    }

    public async Task Executar(Guid pedidoId, Guid pagamentoId)
    {
        var pedido = await _repository.ConsultarPorId(pedidoId);

        if (pedido is null)
            throw new ApplicationException($"Pedido de id '{pedidoId}' não encontrado!");

        pedido.Pagar(pagamentoId);

        _repository.Atualizar(pedido);

        await _repository.UnitOfWork.Commit();
    }
}
