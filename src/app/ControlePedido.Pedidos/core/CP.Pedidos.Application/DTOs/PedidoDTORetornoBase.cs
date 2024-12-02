using System.Text.Json.Serialization;
using CP.Pedidos.CrossCutting;
using CP.Pedidos.Domain.Entities;

namespace CP.Pedidos.Application.DTOs;

public abstract class PedidoDTORetornoBase
{

    [JsonPropertyName("clienteId")]
    public string ClienteId { get; set; }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    public PedidoDTORetornoBase()
    {
    }

    public PedidoDTORetornoBase(Pedido pedido)
    {
        Id = pedido.Id;
        Valor = pedido.Valor;
        ClienteId = pedido.ClienteId != Guid.Empty && pedido.ClienteId != null ? pedido.ClienteId.ToString() : "Cliente não informado";
        Status = pedido.RetornarStatusAtual().GetDescription();
    }
}
