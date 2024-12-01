using System;
using System.Text.Json.Serialization;

namespace CP.Pedidos.Infra.Models.Results;

public class ApiPagamentoErrorResult
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("errors")]
    public ApiPagamentoError Errors { get; set; }

    public bool HasError(string error){
        return Errors?.Message?.Any(m => m == error) ?? false;
    }
}

public class ApiPagamentoError
{
    [JsonPropertyName("Mensagens")]
    public IEnumerable<string> Message { get; set; }
}
