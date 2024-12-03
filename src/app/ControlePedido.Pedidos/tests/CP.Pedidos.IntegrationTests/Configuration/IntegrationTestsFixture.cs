using System.Text.Json;
using ControlePedido.Infra;
using CP.Pedidos.Domain.Adapters.MessageBus;
using CP.Pedidos.Domain.Adapters.Repositories;
using CP.Pedidos.Domain.Entities;
using CP.Pedidos.Domain.ValueObjects;
using CP.Pedidos.Infra.Communications;
using CP.Pedidos.Infra.Models.Request;
using CP.Pedidos.Infra.Models.Results;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using static CP.Pedidos.Domain.Entities.Pedido;

namespace CP.Pedidos.IntegrationTests;

public class IntegrationTestFixture : IDisposable
{
    public WebApplicationFactory<Program> Factory { get; }
    public HttpClient Client { get; private set; }
    public IPedidoRepository repository { get; private set; }
    public ControlePedidoContext context { get; private set; }

    public IntegrationTestFixture()
    {
        Factory = new WebApplicationFactory<Program>()
         .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                   {
                       Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
                       context.HostingEnvironment.EnvironmentName = "Testing";
                   });

                builder.ConfigureServices(async services =>
                {
                    services.AdicionarMockApiPagamento();

                    services.RemoverServicoInjetado<IMessageBus>();
                    services.AddScoped(s => new Mock<IMessageBus>().Object);

                    services.RemoverServicoInjetado<DbContextOptions<ControlePedidoContext>>();
                    services.AddDbContext<ControlePedidoContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });

                    var serviceProvider = services.BuildServiceProvider();
                    context = serviceProvider.GetService<ControlePedidoContext>();
                    context.Database.EnsureCreated();

                    repository = serviceProvider.GetService<IPedidoRepository>();
                });
            });

        Client = Factory.CreateClient();
    }

    public void AdicionarMockService(Action<IServiceCollection> configureServices)
    {
        Factory.WithWebHostBuilder(b => b.ConfigureServices(configureServices));
        Client = Factory.CreateClient();

    }

    public async Task TestarRequisicaoComErro(HttpResponseMessage response, List<string> erros)
    {
        var dados = await response.Content.ReadAsStringAsync();
        var errorDetail = JsonSerializer.Deserialize<ValidationProblemDetails>(dados);

        new ValidationProblemDetails(new Dictionary<string, string[]> {
                { "Mensagens", erros.ToArray() }
            });

        errorDetail.Errors["Mensagens"].Should().BeEquivalentTo(erros);
    }

    public void Dispose()
    {
        Client.Dispose();
        Factory.Dispose();
    }

    internal Pedido CriarPedido()
    {
        var itens = new List<PedidoItem>{
            new PedidoItem(Guid.NewGuid(), "Teste produto", "Descricao do teste do produto", 100, new Imagem("http://teste.com", "png", "teste"))
        };
        return PedidoFactory.Criar(itens);
    }
}

public static class FixtureExtensions
{
    public static IServiceCollection AdicionarMockApiPagamento(this IServiceCollection services)
    {

        var pagamentoApiMock = new Mock<IPagamentoApi>();
        pagamentoApiMock.Setup(p => p.GerarQrCode(It.IsAny<GerarQrCodeRequest>(), It.IsAny<Guid>()))
                        .ReturnsAsync(new GerarQrCodeResult
                        {
                            QrCode = "00020101021243650016COM.MERCADOLIBRE020130636e5ad12fa-79be-4c57-b016-f5092fc9ed3e5204000053039865802BR5909Test Test6009SAO PAULO62070503***63046A2E"
                        });

        RemoverServicoInjetado<IPagamentoApi>(services);
        services.AddScoped(s => pagamentoApiMock.Object);
        return services;
    }

    public static IServiceCollection RemoverServicoInjetado<T>(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
        return services;
    }
}

