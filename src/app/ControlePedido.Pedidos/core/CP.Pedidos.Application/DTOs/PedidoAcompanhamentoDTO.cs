using System.ComponentModel;
using System.Text.Json.Serialization;
using CP.Pedidos.CrossCutting;
using CP.Pedidos.Domain.Entities;

namespace CP.Pedidos.Application.DTOs;

[DisplayName("AcompanhamentoPedido")]
public class AcompanhamentoPedidoDTO : PedidoDTORetornoBase
{
    [JsonPropertyName("codigoPagamento")]
    public Guid? CodigoPagamento { get; set; }

    public AcompanhamentoPedidoDTO()
    {
    }

    public AcompanhamentoPedidoDTO(Pedido pedido) : base(pedido)
    {
        CodigoPagamento =  string.IsNullOrEmpty(pedido.PagamentoId) ? null : Guid.Parse(pedido.PagamentoId);
        Status = pedido.RetornarStatusAtual().GetDescription();
    }
}
