namespace CP.Pedidos.Application.UseCases.Pedidos
{
    public interface IIniciarPreparoPedidoUseCase
	{
		Task Executar(Guid pedidoId);
	}
}

