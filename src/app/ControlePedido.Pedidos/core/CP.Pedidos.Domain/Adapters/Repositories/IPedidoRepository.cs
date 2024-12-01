using CP.Pedidos.Domain.Base;
using CP.Pedidos.Domain.Entities;
using CP.Pedidos.Domain.Enums;

namespace CP.Pedidos.Domain.Adapters.Repositories
{
    public interface IPedidoRepository : IRepository<Pedido>
    {
        void Criar(Pedido pedido);
        void Atualizar(Pedido pedido);
        Task<Pedido?> ConsultarPorId(Guid pedidoId);
        Task<ICollection<Pedido>> ListarPedidos(StatusPedido? status);
    }
}

