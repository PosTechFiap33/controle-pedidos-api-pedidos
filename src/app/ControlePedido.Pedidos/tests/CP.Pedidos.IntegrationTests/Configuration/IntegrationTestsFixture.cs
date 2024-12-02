using System.Text.Json;
using ControlePedido.Infra;
using CP.Pedidos.Domain.Adapters.MessageBus;
using CP.Pedidos.Domain.Adapters.Repositories;
using CP.Pedidos.Domain.Entities;
using CP.Pedidos.Domain.ValueObjects;
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
    public HttpClient Client { get; private set;}
    public IPedidoRepository repository { get; private set; }
    public ControlePedidoContext context { get; private set; }

    public IntegrationTestFixture()
    {
        Factory = new WebApplicationFactory<Program>()
         .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                   {
                       context.HostingEnvironment.EnvironmentName = "Testing";
                   });

                builder.ConfigureServices(async services =>
                {
                    RemoverServicoInjetado<IMessageBus>(services);
                    services.AddScoped(s => new Mock<IMessageBus>().Object);
                    // Remove o contexto de banco de dados existente, se houver
                   
                    RemoverServicoInjetado<ControlePedidoContext>(services);
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

    public void RemoverServicoInjetado<T>(IServiceCollection services){
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }

    internal Pedido CriarPedido()
    {
        var itens = new List<PedidoItem>{
            new PedidoItem(Guid.NewGuid(), "Teste produto", "Descricao do teste do produto", 100, new Imagem("http://teste.com", "png", "teste"))
        };
        return PedidoFactory.Criar(itens);
    }
}

