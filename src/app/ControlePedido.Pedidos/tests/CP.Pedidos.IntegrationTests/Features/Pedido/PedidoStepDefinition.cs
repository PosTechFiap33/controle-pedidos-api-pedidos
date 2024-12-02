using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bogus;
using CP.Pedidos.Application.DTOs;
using CP.Pedidos.Domain.Entities;
using CP.Pedidos.Domain.ValueObjects;
using CP.Pedidos.IntegrationTests;
using FluentAssertions;
using TechTalk.SpecFlow;
using Xunit;

namespace ControlePedido.IntegrationTests;

[Binding]
public class PedidoStepDefinitions : IClassFixture<IntegrationTestFixture>
{
    private readonly CriarPedidoDTO _pedido;
    private HttpClient Client
    {
        get
        {
            return _fixture.Client;
        }
    }
    private readonly string _rota = "api/pedido";
    private HttpResponseMessage _response;
    private PedidoCriadoDTO _pedidoCriado;
    private AcompanhamentoPedidoDTO _acompanhamentoPedido;
    private string _codigoTransacaoPagamento;
    private readonly IntegrationTestFixture _fixture;
    private List<Pedido> _pedidos;
    // private Pedido _pedidoFinalizado;
    // private Pedido _pedidoPago;

    public PedidoStepDefinitions(IntegrationTestFixture fixture)
    {
        _pedido = new CriarPedidoDTO();
        _fixture = fixture;
    }

    [Given(@"que eu iforme o id do cliente ""(.*)""")]
    public void GivenQueEuInformeIdCliente(string clientId)
    {
        _pedido.ClienteId = Guid.Parse(clientId);
    }


    [Given(@"que eu ja tenha um pedido finalizado")]
    public async Task Givenqueeujatenhaumpedidofinalizado()
    {
        var _pedidoFinalizado = _fixture.CriarPedido();
        _pedidoFinalizado.Pagar(Guid.NewGuid());
        _pedidoFinalizado.IniciarPreparo();
        _pedidoFinalizado.FinalizarPreparo();

        _fixture.context.Pedido.Add(_pedidoFinalizado);

        await _fixture.context.SaveChangesAsync();

        _pedidoCriado = new PedidoCriadoDTO { Pedido = new PedidoDTO(_pedidoFinalizado) };
    }


    [Given(@"que eu ja tenha um pedido pago")]
    public async Task Givenqueeujatenhaumpedidopago()
    {
        var _pedidoPago = _fixture.CriarPedido();
        _pedidoPago.Pagar(Guid.NewGuid());
        _fixture.context.Pedido.Add(_pedidoPago);
        await _fixture.context.SaveChangesAsync();
        _pedidoCriado = new PedidoCriadoDTO { Pedido = new PedidoDTO(_pedidoPago) };

    }

    [Given(@"que eu ja tenha um pedido iniciado")]
    public async Task Givenqueeujatenhaumpedidoiniciado()
    {
        var pedidoIniciado = _fixture.CriarPedido();
        pedidoIniciado.Pagar(Guid.NewGuid());
        pedidoIniciado.IniciarPreparo();
        _fixture.context.Pedido.Add(pedidoIniciado);
        await _fixture.context.SaveChangesAsync();
        _pedidoCriado = new PedidoCriadoDTO { Pedido = new PedidoDTO(pedidoIniciado) };
    }

    [Given(@"que eu adicione o produto de valor (.*)")]
    public async Task Givenqueeuadicioneoprodutodeidevalor(decimal value)
    {
        var faker = new Faker("pt_BR");

        var nomeProduto = faker.Commerce.ProductName();
        var descricaoProduto = faker.Lorem.Sentence();

        _pedido.Itens.Add(new PedidoItemDTO
        {
            Nome = nomeProduto,
            Descricao = descricaoProduto,
            ProdutoId = Guid.NewGuid(),
            Preco = value,
            Imagem = new Imagem("http://teste.com", "png", "teste")
        });
    }


    [Given(@"que eu tenha pedidos cadastrados")]
    public async Task GivenQueEuTenhaPedidosCadastrados()
    {
        var pedidosRemover = _fixture.context.Pedido.ToList();
        _fixture.context.Pedido.RemoveRange(pedidosRemover);
        _fixture.context.SaveChanges();

        var pedidoCriado = _fixture.CriarPedido();

        var pedidoPago = _fixture.CriarPedido();
        pedidoPago.Pagar(Guid.NewGuid());

        var pedidoIniciado = _fixture.CriarPedido();
        pedidoIniciado.Pagar(Guid.NewGuid());
        pedidoIniciado.IniciarPreparo();

        var pedidoFinalizado = _fixture.CriarPedido();
        pedidoFinalizado.Pagar(Guid.NewGuid());
        pedidoFinalizado.IniciarPreparo();
        pedidoFinalizado.FinalizarPreparo();

        var pedidoEntregue = _fixture.CriarPedido();
        pedidoEntregue.Pagar(Guid.NewGuid());
        pedidoEntregue.IniciarPreparo();
        pedidoEntregue.FinalizarPreparo();
        pedidoEntregue.Finalizar();

        _pedidos = new List<Pedido>
        {
           pedidoCriado,
           pedidoPago,
           pedidoIniciado,
           pedidoFinalizado,
           pedidoEntregue
        };

        _fixture.context.Pedido.AddRange(_pedidos);
        await _fixture.context.SaveChangesAsync();
    }

    [When(@"eu fizer uma requisicao para gerar o pedido")]
    public async Task Wheneufizerumarequisicaoparageraropedido()
    {
        _response = await Client.PostAsJsonAsync(_rota, _pedido);
        _pedidoCriado = await RecuperarPedidoDaResposta();
    }

    [When(@"eu fizer uma requisicao para iniciar o preparo")]
    public async Task Wheneufizerumarequisicaoparainiciaropreparo()
    {
        _response = await Client.PatchAsync($"{_rota}/{_pedidoCriado.Pedido.Id}/iniciar-preparo", null);
    }

    [When(@"eu fizer uma requisicao para finalizar o prepado do pedido")]
    public async Task Wheneufizerumarequisicaoparafinalizaroprepadodopedido()
    {
        _response = await Client.PatchAsync($"{_rota}/{_pedidoCriado.Pedido.Id}/finalizar-preparo", null);
    }

    [When(@"eu fizer o pagamento manual do pedido criado")]
    public async Task Wheneufizeropagamentomanualdopedidocriado()
    {
        //TODO pensar em como melhorar pois hj isso vem da fila, como simular a fila?
        var pedido = await _fixture.repository
                                   .ConsultarPorId(_pedidoCriado.Pedido.Id);
        pedido.Pagar(Guid.NewGuid());
        _fixture.repository.Atualizar(pedido);
        await _fixture.repository.UnitOfWork.Commit();
    }

    [When(@"eu fizer uma requisicao para realizar a entrega do pedido")]
    public async Task Wheneufizerumarequisicaopararealizaraentregadopedido()
    {
        _response = await Client.PatchAsync($"{_rota}/{_pedidoCriado.Pedido.Id}/entregar", null);
    }

    [When(@"eu fizer uma requisicao listar os pedidos")]
    public async Task Wheneufizerumarequisicaolistarospedidos()
    {
        _response = await Client.GetAsync($"{_rota}");
    }

    [Then(@"o status code deve ser (.*)")]
    public void Thenostatuscodedeveser(HttpStatusCode statusCode)
    {
        _response.StatusCode.Should().Be(statusCode);
    }

    [Then(@"os dados do pedido estejam validos")]
    public async Task GivenOsDadosPedidoEstejamValidos()
    {
        _pedidoCriado.Pedido.Id.Should().NotBe(Guid.Empty);
        _pedidoCriado.Pedido.Itens.Should().Contain(p => _pedido.Itens.Any(x => x.ProdutoId.ToString() == p.ProdutoId.ToString()));
        _pedidoCriado.Pedido.Status.Should().Be("Criado");
        _pedidoCriado.QRCodePagamento.Should().NotBeEmpty();
    }

    [Then(@"o id vinculado no pedido deve ser ""(.*)""")]
    public void Thenoidvinculadonopedidodeveser(string clientId)
    {
        _pedidoCriado.Pedido.ClienteId.Should().Be(clientId);
    }

    [Then(@"o valor do pedido deve ser (.*)")]
    public void Thenovalordopedidodeveser(decimal valor)
    {
        _pedidoCriado.Pedido.Valor.Should().Be(valor);
    }

    [Then(@"o status do pedido deve ser ""(.*)""")]
    public async Task Thenostatusdopedidodeveser(string status)
    {
        var response = await Client.GetAsync($"{_rota}/{_pedidoCriado.Pedido.Id}/acompanhar");
        var dados = await response.Content.ReadAsStringAsync();
        _acompanhamentoPedido = JsonSerializer.Deserialize<AcompanhamentoPedidoDTO>(dados);
        _acompanhamentoPedido.ClienteId.Should().Be(_pedidoCriado.Pedido.ClienteId);
        _acompanhamentoPedido.Status.Should().Be(status);
        _acompanhamentoPedido.Valor.Should().Be(_pedidoCriado.Pedido.Valor);
    }

    [Then(@"os dados do pagamento devem estar vazios")]
    public void Thenosdadosdopagamentodevemestarvazios()
    {
        _acompanhamentoPedido.CodigoPagamento.Should().BeNull();
    }

    [Then(@"os dados do pagamento devem estar preenchidos")]
    public void Thenosdadosdopagamentodevemestarpreenchidos()
    {
        _acompanhamentoPedido.CodigoPagamento.Should().NotBeEmpty();
    }

    [Then(@"deve ser exibida a mensagem de erro ""(.*)""")]
    public async Task Thendeveserexibidaamensagemdeerro(string erro)
    {
        await _fixture.TestarRequisicaoComErro(_response, new List<string> { erro });
    }

    [Then(@"deve ser exibida a lista dos pedidos")]
    public async Task Thendeveserexibidaalistadospedidos()
    {
        var statusDesejados = new List<string> {
               "Pronto",
               "Em preparacao",
               "Recebido"
             };

        var dados = await _response.Content.ReadAsStringAsync();

        var pedidos = JsonSerializer.Deserialize<List<PedidoDTO>>(dados, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true
        });

        pedidos.Should().NotBeEmpty();
        pedidos[0].Status.Should().Be("Pronto");
        pedidos[0].Should().BeEquivalentTo(pedidos.Where(p => p.Status == "Pronto").FirstOrDefault());

        pedidos[1].Status.Should().Be("Em preparacao");
        pedidos[1].Should().BeEquivalentTo(pedidos.Where(p => p.Status == "Em preparacao").FirstOrDefault());

        pedidos[2].Status.Should().Be("Recebido");
        pedidos[2].Should().BeEquivalentTo(pedidos.Where(p => p.Status == "Recebido").FirstOrDefault());
    }

    private async Task<PedidoCriadoDTO> RecuperarPedidoDaResposta()
    {
        try
        {
            var dados = await _response.Content.ReadAsStringAsync();
            var teste = JsonSerializer.Deserialize<PedidoCriadoDTO>(dados, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true
            });
            return teste;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
