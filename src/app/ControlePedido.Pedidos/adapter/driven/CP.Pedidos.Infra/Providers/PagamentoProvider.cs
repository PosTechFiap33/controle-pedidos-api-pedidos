using System.Text.Json;
using CP.Pedidos.Domain.Adapters.Providers;
using CP.Pedidos.Domain.Base;
using CP.Pedidos.Domain.Entities;
using CP.Pedidos.Infra.Communications;
using CP.Pedidos.Infra.Models.Request;
using CP.Pedidos.Infra.Models.Results;
using Microsoft.Extensions.Logging;
using Refit;

namespace CP.Pedidos.Infra.Providers;

public class PagamentoProvider : IPagamentoProvider
{
    private readonly IPagamentoApi _pagamentoApi;
    private readonly ILogger<PagamentoProvider> _logger;

    public PagamentoProvider(IPagamentoApi pagamentoApi, ILogger<PagamentoProvider> logger)
    {
        _pagamentoApi = pagamentoApi;
        _logger = logger;
    }

    public async Task<string> GerarQrCode(Pedido pedido)
    {
        try
        {
            _logger.LogInformation("Iniciando o processo de geração de QR Code para o pedido {PedidoId}.", pedido.Id);

            var qrCodeItens = pedido.Itens.Select(x => new Item(x.Nome, x.Descricao, x.Preco, 1));
            var qrCodeRequest = new GerarQrCodeRequest(qrCodeItens);

            var qrCodeResult = await _pagamentoApi.GerarQrCode(qrCodeRequest, pedido.Id);

            _logger.LogInformation("QR Code gerado com sucesso para o pedido {PedidoId}.", pedido.Id);

            return qrCodeResult.QrCode;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Erro ao tentar gerar QR Code para o pedido {PedidoId} via Mercado Pago.", pedido.Id);

            var mensagemByPass = "Não foi possível comunicar com o sistema de pagamento para gerar o qrcode, utilize a rota de pagamento manual para prosseguir com seu pedido!";

            var error = JsonSerializer.Deserialize<ApiPagamentoErrorResult>(ex.Content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true,
            });

            if (error is not null && error.HasError(mensagemByPass))
            {
                _logger.LogWarning("Erro específico do sistema de pagamento identificado, retornando mensagem de bypass para o pedido {PedidoId}.", pedido.Id);
                return mensagemByPass;
            }

            throw new IntegrationExceptions($"Erro ao chamar api de pagamento: {error.Title}. Message: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao tentar gerar QR Code para o pedido {PedidoId}.", pedido.Id);
            throw new ApplicationException($"Erro inesperado ao tentar gerar QR Code para o pedido {pedido.Id}.", ex);
        }
    }
}
