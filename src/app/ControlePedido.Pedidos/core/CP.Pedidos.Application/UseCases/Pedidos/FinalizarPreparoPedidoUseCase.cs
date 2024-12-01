using CP.Pedidos.Domain.Adapters.Repositories;
using CP.Pedidos.Domain.Base;

namespace CP.Pedidos.Application.UseCases.Pedidos
{
    public class FinalizarPreparoPedidoUseCase : IFinalizarPreparoPedidoUseCase
	{
        private readonly IPedidoRepository _repository;

        public FinalizarPreparoPedidoUseCase(IPedidoRepository pedidoRepository)
        {
            _repository = pedidoRepository;
        }

        public async Task Executar(Guid pedidoId)
        {
            var pedido = await _repository.ConsultarPorId(pedidoId);

            if (pedido is null)
                throw new DomainException("Não foi encontrado um pedido para o código informado!");

            pedido.FinalizarPreparo();

            _repository.Atualizar(pedido);

            await _repository.UnitOfWork.Commit();
        }
    }
}

