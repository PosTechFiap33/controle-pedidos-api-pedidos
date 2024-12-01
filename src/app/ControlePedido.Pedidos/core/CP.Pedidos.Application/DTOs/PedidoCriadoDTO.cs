using System.ComponentModel;
using System.Text.Json.Serialization;
using CP.Pedidos.Domain.Entities;

namespace CP.Pedidos.Application.DTOs
{
    [DisplayName("PedidoCriado")]
    public class PedidoCriadoDTO
    {
        [JsonPropertyName("pedido")]
        public PedidoDTO Pedido { get; set; }

        [JsonPropertyName("qrCodePagamento")]
        public string QRCodePagamento { get; set; }

        public PedidoCriadoDTO()
        {
        } 
    }
}

