using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CP.Pedidos.Domain.ValueObjects;

namespace CP.Pedidos.Application.DTOs
{
    [DisplayName("ItemPedido")]
    public class PedidoItemDTO
    {
        [Required(ErrorMessage = "Campo {0} obrigatorio")]
        [JsonPropertyName("produtoId")]
        public Guid ProdutoId { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("preco")]
        public decimal Preco { get; set; }

        [JsonPropertyName("imagem")]
        public Imagem Imagem { get; set; }
    }

    [DisplayName("CriarPedido")]
    public class CriarPedidoDTO
    {
        public Guid? ClienteId { get; set; }

        [Required(ErrorMessage = "Campo {0} obrigatorio")]
        public ICollection<PedidoItemDTO> Itens { get; set; }

        public CriarPedidoDTO()
        {
            Itens = new List<PedidoItemDTO>();
        }
    }
}

