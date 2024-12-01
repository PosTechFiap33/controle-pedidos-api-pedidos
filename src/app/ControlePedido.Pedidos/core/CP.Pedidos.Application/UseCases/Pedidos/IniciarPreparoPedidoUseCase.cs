using CP.Pedidos.Domain.Adapters.Repositories;
using CP.Pedidos.Domain.Base;

namespace CP.Pedidos.Application.UseCases.Pedidos
{
    public class IniciarPreparoPedidoUseCase : IIniciarPreparoPedidoUseCase
	{
		private readonly IPedidoRepository _repository;

        public IniciarPreparoPedidoUseCase(IPedidoRepository repository)
        {
            _repository = repository;
        }

        public async Task Executar(Guid pedidoId)
        {
            var pedido = await _repository.ConsultarPorId(pedidoId);

            if (pedido is null)
                throw new DomainException("Não foi encontrado um pedido para o código informado!");

            pedido.IniciarPreparo();

            _repository.Atualizar(pedido);

            await _repository.UnitOfWork.Commit();
        }
    }
}

