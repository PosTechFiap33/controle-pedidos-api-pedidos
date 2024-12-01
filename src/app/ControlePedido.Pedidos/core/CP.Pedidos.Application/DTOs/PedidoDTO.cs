using System.ComponentModel;
using System.Text.Json.Serialization;
using CP.Pedidos.CrossCutting;
using CP.Pedidos.Domain.Entities;

namespace CP.Pedidos.Application.DTOs
{
    [DisplayName("Pedido")]
    public class PedidoDTO
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }

        [JsonPropertyName("clienteId")]
        public string ClienteId { get; set; }

        [JsonPropertyName("itens")]
        public IEnumerable<PedidoItemDTO> Itens { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("dataHora")]
        public string DataHora { get; set; }

        public PedidoDTO()
        {
        }

        public PedidoDTO(Pedido pedido)
        {
            Id = pedido.Id;
            Valor = pedido.Valor;
            ClienteId = pedido.ClienteId?.ToString() ?? "Cliente não informado";
            Itens = pedido.Itens.Select(item => new PedidoItemDTO
            {
                ProdutoId = item.ProdutoId,
                Nome = item.Nome,
                Preco = item.Preco,
                Imagem = item.Imagem
            }).ToList();
            Status = pedido.RetornarStatusAtual().GetDescription();
            DataHora = pedido.RetornarDataHora();
        }
    }
}

