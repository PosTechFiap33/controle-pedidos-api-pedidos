using System.ComponentModel;
using System.Text.Json.Serialization;
using CP.Pedidos.CrossCutting;
using CP.Pedidos.Domain.Entities;

namespace CP.Pedidos.Application.DTOs;

[DisplayName("AcompanhamentoPedido")]
public class AcompanhamentoPedidoDTO
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("cpfCliente")]
    public string? ClientId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }

    [JsonPropertyName("codigoPagamento")]
    public Guid? CodigoPagamento { get; set; }

    public AcompanhamentoPedidoDTO()
    {
    }

    public AcompanhamentoPedidoDTO(Pedido pedido)
    {
        Id = pedido.Id;
        Valor = pedido.Valor;

        if (pedido.ClienteId is not null)
            ClientId = pedido.ClienteId.ToString();
        else
            ClientId = "Cliente não informado!";

        CodigoPagamento = pedido.PagamentoId;
     
        Status = pedido.RetornarStatusAtual().GetDescription();
    }
}
