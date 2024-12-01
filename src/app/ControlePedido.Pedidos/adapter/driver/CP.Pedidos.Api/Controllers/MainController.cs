using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace CP.Pedidos.Api.Controllers;

public abstract class MainController : ControllerBase
{
    protected ICollection<string> Erros = new List<string>();
    protected readonly ILogger _logger;

    protected MainController(ILogger logger)
    {
        _logger = logger;
    }

    protected ActionResult CustomResponse(object? result = null, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        ObjectResult response;

        if (OperacaoValida())
            response = StatusCode((int)statusCode, result);
        else
        {
            var status = statusCode is HttpStatusCode.OK ? HttpStatusCode.BadRequest : statusCode;
            response = StatusCode((int)status, RecuperarErros());
        }

        string serializedValue = JsonSerializer.Serialize(response.Value);
        _logger.LogInformation("Resposta devolvida para o client - Status: {StatusCode} - Value: {Value}", response.StatusCode, serializedValue);

        return response;
    }

    protected bool OperacaoValida()
    {
        return !Erros.Any();
    }

    private ValidationProblemDetails RecuperarErros()
    {
        return new ValidationProblemDetails(new Dictionary<string, string[]> {
                {
                    "Mensagens", Erros.ToArray()
                }
            });
    }

    protected void AdicionarErroProcessamento(string erro)
    {
        Erros.Add(erro);
    }

    protected void LimparErrosProcessamento()
    {
        Erros.Clear();
    }
}
