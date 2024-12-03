using CP.Pedidos.Domain.Base;
using CP.Pedidos.Domain.Entities;
using CP.Pedidos.Domain.Enums;
using CP.Pedidos.Domain.ValueObjects;
using Xunit;
using static CP.Pedidos.Domain.Entities.Pedido;

namespace CP.Pedidos.UnitTests.Domain.Entities;

public class PedidoTests
{
    private ICollection<PedidoItem> itens;
    private readonly Guid _clientId;

    public PedidoTests()
    {
        var imagem = new Imagem("http://teste", "jpg", "teste");

        itens = new List<PedidoItem> {
               new PedidoItem(Guid.NewGuid(), "teste", "descricao", 10, imagem)
            };

        _clientId = Guid.NewGuid();
    }

    [Fact(DisplayName = "Deve retornar mensagem 'O Pedido deve conter pelo nenos 1 item!' ao instanciar um pedido sem item")]
    public void Criar_DeveLancarExcecaoQuandoCriarPedidoSemItens()
    {
        var excecao = Assert.Throws<DomainException>(() => PedidoFactory.Criar(new List<PedidoItem>()));
        Assert.Equal("O Pedido deve conter pelo nenos 1 item!", excecao.Message);
    }

    [Fact(DisplayName = "Deve retornar mensagem 'O valor do pedido deve ser maior que 0!' ao instanciar um pedido com valor menor igual a 0")]
    public void Criar_DeveLancarExcecaoQuandoValorDoPedidoForMenorIgualZero()
    {
        var imagem = new Imagem("http://teste", "jpg", "teste");

        var excecao = Assert.Throws<DomainException>(() => {
            var itens = new List<PedidoItem> {
                    new PedidoItem(Guid.NewGuid(), "teste", "desc", 0, imagem)
                };
                PedidoFactory.Criar(itens);
            });
            
        Assert.Equal("O preço deve ser maior do que 0.", excecao.Message);
    }

    [Fact(DisplayName = "Deve criar um pedido com sucesso")]
    public void Criar_DeveCriarPedidoComSucesso()
    {
        var pedido = PedidoFactory.Criar(itens, _clientId);

        Assert.Equal(_clientId, pedido.ClienteId);
        Assert.Equal(itens, pedido.Itens);
    }

    [Fact(DisplayName = "Deve realizar um pagamento de pedido com sucesso")]
    public void Pagar_DeveCriarPagamentoQuandoCodigoTransacaoEhVazio()
    {
        var pedido = PedidoFactory.Criar(itens, _clientId);

        pedido.Pagar(Guid.NewGuid());

        Assert.NotNull(pedido.PagamentoId);
        Assert.Equal(StatusPedido.RECEBIDO, pedido.RetornarStatusAtual());
    }

    [Theory(DisplayName = "Deve alterar o status apenas uma vez ao pagar pedido!")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Pagar_DeveInserirStatusApenasUmaVez(int quantidadeExecucao)
    {
        var pedido = PedidoFactory.Criar(itens, _clientId);

        for (var i = 0; i < quantidadeExecucao; i++)
            pedido.Pagar(Guid.NewGuid());

        Assert.Equal(2, pedido.Status.Count);
    }

    [Fact(DisplayName = "Deve retornar mensagem de pagamento nao realizado ao iniciar o preparo")]
    public void IniciarPreparo_DeveRetornarMensagemPagamentoNaoRealizado()
    {
        var pedido = PedidoFactory.Criar(itens, _clientId);

        var excecao = Assert.Throws<DomainException>(() => pedido.IniciarPreparo());

        Assert.Equal("Não foi realizado o pagamento para o pedido informado!", excecao.Message);
        Assert.Equal(StatusPedido.CRIADO, pedido.RetornarStatusAtual());
    }

    [Fact(DisplayName = "Deve iniciar o preparo do pedido quando pagamento realizado")]
    public void IniciarPreparo_DeveAtualizarStatusParaEmPreparacaoQuandoPagamentoRealizado()
    {
        var pedido = PedidoFactory.Criar(itens, _clientId);
        pedido.Pagar(Guid.NewGuid());

        pedido.IniciarPreparo();

        Assert.Equal(StatusPedido.EM_PREPARACAO, pedido.RetornarStatusAtual());
    }

    [Fact(DisplayName = "Deve retornar mensagem 'Não foi possível finalizar o preparo do pedido pois o preparo não foi iniciado!' ao finalizar preparo!")]
    public void FinalizarPreparo_DeveRetornarMensagemPreparoPedidoNaoFinalizado()
    {
        var pedido = PedidoFactory.Criar(itens, _clientId);
        pedido.Pagar(Guid.NewGuid());

        var excecao = Assert.Throws<DomainException>(() => pedido.FinalizarPreparo());

        Assert.Equal("Não foi possível finalizar o preparo do pedido pois o preparo não foi iniciado!", excecao.Message);
        Assert.Equal(StatusPedido.RECEBIDO, pedido.RetornarStatusAtual());
    }

    [Fact(DisplayName = "Deve finalizar o preparo do pedido com sucesso apos preparo iniciado!")]
    public void FinalizarPreparo_DeveAtualizarStatusParaProntoQuandoPreparoIniciado()
    {
        var pedido = PedidoFactory.Criar(itens, _clientId);
        pedido.Pagar(Guid.NewGuid());
        pedido.IniciarPreparo();

        pedido.FinalizarPreparo();

        Assert.Equal(StatusPedido.PRONTO, pedido.RetornarStatusAtual());
    }

    [Fact(DisplayName = "Deve retornar mensagem 'Não foi possível finalizar o pedido pois o preparo não foi finalizado!' ao finalizar o pedido")]
    public void Finalizar_DeveRetornarMensagemPreparoNaoFinalizado()
    {
        var pedido = PedidoFactory.Criar(itens, _clientId);
        pedido.Pagar(Guid.NewGuid());

        var excecao = Assert.Throws<DomainException>(() => pedido.Finalizar());

        Assert.Equal("Não foi possível finalizar o pedido pois o preparo não foi finalizado!", excecao.Message);
        Assert.Equal(StatusPedido.RECEBIDO, pedido.RetornarStatusAtual());
    }

    [Fact(DisplayName = "Deve finalizar o pedido com sucesso!")]
    public void Finalizar_DeveAtualizarStatusParaFinalizadoQuandoPreparoFinalizado()
    {
        var pedido = PedidoFactory.Criar(itens, _clientId);
        pedido.Pagar(Guid.NewGuid());
        pedido.IniciarPreparo();
        pedido.FinalizarPreparo();

        pedido.Finalizar();

        Assert.Equal(StatusPedido.FINALIZADO, pedido.RetornarStatusAtual());
    }
}