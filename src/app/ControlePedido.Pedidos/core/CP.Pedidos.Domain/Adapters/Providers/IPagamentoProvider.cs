using CP.Pedidos.Domain.Entities;

namespace CP.Pedidos.Domain.Adapters.Providers;

public interface IPagamentoProvider
{
    Task<string> GerarQrCode(Pedido pedido);
}
