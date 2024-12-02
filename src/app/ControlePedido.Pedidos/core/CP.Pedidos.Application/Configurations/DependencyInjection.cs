using System.Reflection;
using ControlePedido.Application.UseCases.Pedidos;
using CP.Pedidos.Application.UseCases.Pedidos;
using CP.Pedidos.Domain.UseCases;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CP.Pedidos.Application.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddTransient<ICriarPedidoUseCase, CriarPedidoUseCase>();
        services.AddTransient<IPagarPedidoManualmenteUseCase, PagarPedidoManualmenteUseCase>();
        services.AddTransient<IListarPedidoUseCase, ListarPedidoUseCase>();
        services.AddTransient<IIniciarPreparoPedidoUseCase, IniciarPreparoPedidoUseCase>();
        services.AddTransient<IFinalizarPreparoPedidoUseCase, FinalizarPreparoPedidoUseCase>();
        services.AddTransient<IEntregarPedidoUseCase, EntregarPedidoUseCase>();
        services.AddTransient<IAcompanharPedidoUseCase, AcompanharPedidoUseCase>();
        services.AddScoped<IPagarPedidoUseCase, PagarPedidoUseCase>();
        
        return services;
    }
}
