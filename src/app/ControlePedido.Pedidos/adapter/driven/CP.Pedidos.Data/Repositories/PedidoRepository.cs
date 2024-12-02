using CP.Pedidos.Domain.Adapters.Repositories;
using CP.Pedidos.Domain.Base;
using CP.Pedidos.Domain.Entities;
using CP.Pedidos.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ControlePedido.Infra.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly ControlePedidoContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public PedidoRepository(ControlePedidoContext context)
        {
            _context = context;
        }

        public void Criar(Pedido pedido)
        {
            _context.Entry(pedido).State = EntityState.Added;

            foreach (var item in pedido.Itens)
                _context.Entry(item).State = EntityState.Added;

            foreach (var status in pedido.Status)
                _context.Entry(status).State = EntityState.Added;
        }

        //TODO - avaliar maneiras melhores de fazer
        public void Atualizar(Pedido pedido)
        {
          //  _context.Pedido.Attach(pedido);

            _context.Entry(pedido).State = EntityState.Modified; 

            foreach (var status in pedido.Status)
            {
                if (status.PedidoId == Guid.Empty)
                    _context.Entry(status).State = EntityState.Added;
                else
                    _context.Entry(status).State = EntityState.Modified;
            }
        }

        public async Task<Pedido?> ConsultarPorId(Guid pedidoId)
        {
            return await _context.Pedido
                           .AsNoTracking()
                           .Include(p => p.Status)
                           .FirstOrDefaultAsync(p => p.Id == pedidoId);
        }

        public async Task<ICollection<Pedido>> ListarPedidos(StatusPedido? status)
        {
            var statusDesejados = new List<StatusPedido> {
                StatusPedido.PRONTO,
                StatusPedido.EM_PREPARACAO,
                StatusPedido.RECEBIDO
             };

            IQueryable<Pedido> query = _context.Pedido
                                         .AsNoTracking()
                                         .Include(p => p.Itens)
                                         .Include(p => p.Status);

            if (status.HasValue)
            {
                query = query.Where(p => p.Status
                             .OrderByDescending(s => s.DataHora)
                             .FirstOrDefault().Status == status);
            }
            else
            {
                query = query.Where(p =>
                    statusDesejados.Contains(p.Status
                        .OrderByDescending(s => s.DataHora)
                        .FirstOrDefault().Status));

                query = query.OrderBy(p =>
                        p.Status.OrderByDescending(s => s.DataHora)
                                .FirstOrDefault()
                                .DataHora);
            }

            return await query.ToListAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

