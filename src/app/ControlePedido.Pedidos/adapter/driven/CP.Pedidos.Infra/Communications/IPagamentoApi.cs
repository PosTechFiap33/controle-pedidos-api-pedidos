using CP.Pedidos.Infra.Models.Request;
using CP.Pedidos.Infra.Models.Results;
using Refit;

namespace CP.Pedidos.Infra.Communications;

public interface IPagamentoApi
{
    [Post("/pedido/{pedidoId}/qrcode")]
    Task<GerarQrCodeResult> GerarQrCode([Body] GerarQrCodeRequest dados, Guid pedidoId);
}
