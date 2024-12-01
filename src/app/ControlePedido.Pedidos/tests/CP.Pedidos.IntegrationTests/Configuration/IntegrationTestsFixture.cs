using System.Text.Json;
using ControlePedido.Infra;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CP.Pedidos.IntegrationTests;

public class IntegrationTestFixture : IDisposable
{
    public WebApplicationFactory<Program> Factory { get; }
    public HttpClient Client { get; }
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
                    // Remove o contexto de banco de dados existente, se houver
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ControlePedidoContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ControlePedidoContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });

                    var serviceProvider = services.BuildServiceProvider();

                    context = serviceProvider.GetService<ControlePedidoContext>();
                    context.Database.EnsureCreated();
                });
            });

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
}

