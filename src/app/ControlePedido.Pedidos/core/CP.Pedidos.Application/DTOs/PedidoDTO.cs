using System.ComponentModel;
using System.Text.Json.Serialization;
using CP.Pedidos.CrossCutting;
using CP.Pedidos.Domain.Entities;

namespace CP.Pedidos.Application.DTOs
{

    [DisplayName("Pedido")]
    public class PedidoDTO : PedidoDTORetornoBase
    {
        [JsonPropertyName("itens")]
        public IEnumerable<PedidoItemDTO> Itens { get; set; }

        [JsonPropertyName("dataHora")]
        public string DataHora { get; set; }

        public PedidoDTO()
        {
        }

        public PedidoDTO(Pedido pedido) : base(pedido)
        {
            Itens = pedido.Itens.Select(item => new PedidoItemDTO
            {
                ProdutoId = item.ProdutoId,
                Nome = item.Nome,
                Preco = item.Preco,
                Imagem = item.Imagem
            }).ToList();
            DataHora = pedido.RetornarDataHora();
        }
    }
}

