using CP.Pedidos.Application.DTOs;
using CP.Pedidos.Domain.Adapters.Providers;
using CP.Pedidos.Domain.Adapters.Repositories;
using CP.Pedidos.Domain.Entities;
using static CP.Pedidos.Domain.Entities.Pedido;

namespace CP.Pedidos.Application.UseCases.Pedidos
{
    public class CriarPedidoUseCase : ICriarPedidoUseCase
    {
        private readonly IPedidoRepository _repository;
        private readonly IPagamentoProvider _pagamentoProvider;

        public CriarPedidoUseCase(IPedidoRepository repository, 
                                  IPagamentoProvider pagamentoProvider)
        {
            _repository = repository;
            _pagamentoProvider = pagamentoProvider;
        }

        public async Task<PedidoCriadoDTO> Executar(CriarPedidoDTO criarPedidoDTO)
        {
            var itensPedido = new List<PedidoItem>();

            var itens = criarPedidoDTO.Itens.Select(i => new PedidoItem(i.ProdutoId, i.Nome, i.Descricao, i.Preco, i.Imagem));

            itensPedido.AddRange(itens);

            var pedido = PedidoFactory.Criar(itensPedido, criarPedidoDTO.ClienteId);

            await SalvarPedido(pedido);

            var qrCode = await _pagamentoProvider.GerarQrCode(pedido);

            return new PedidoCriadoDTO
            {
                Pedido = new PedidoDTO(pedido),
                QRCodePagamento = qrCode
            };
        }

        private async Task SalvarPedido(Pedido pedido)
        {
            _repository.Criar(pedido);
            await _repository.UnitOfWork.Commit();
        }
    }
}

