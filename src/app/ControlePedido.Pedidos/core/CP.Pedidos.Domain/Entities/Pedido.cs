using CP.Pedidos.Domain.Base;
using CP.Pedidos.Domain.Enums;
using CP.Pedidos.Domain.ValueObjects;

namespace CP.Pedidos.Domain.Entities
{
    public class PedidoItem : Entity
    {
        public Guid PedidoId { get; private set; }
        public Guid ProdutoId { get; private set; }
        public string Nome { get; private set; }
        public string? Descricao { get; private set; }

        public decimal Preco { get; private set; }
        public Imagem Imagem { get; private set; }
        public virtual Pedido Pedido { get; set; }

        protected PedidoItem() { }

        public PedidoItem(Guid produtoId, string nome, string descricao, decimal preco, Imagem? imagem)
        {
            ProdutoId = produtoId;
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
            Imagem = imagem;

            AssertionConcern.AssertArgumentNotEquals(Guid.Empty, ProdutoId, "Codigo do produto não foi informado!");
            AssertionConcern.AssertArgumentNotEmpty(Nome, "O Nome não pode estar vazio!");
            AssertionConcern.AssertArgumentNotEmpty(Descricao, "A Descrição não pode estar vazia!");
            AssertionConcern.AssertArgumentLength(Nome, 100, "O nome não pode ultrapassar 100 caracters!");
            AssertionConcern.AssertGratherThanValue(Preco, 0, "O preço deve ser maior do que 0.");
        }
    }

    public class PedidoStatus : Entity
    {
        public Guid PedidoId { get; private set; }
        public StatusPedido Status { get; private set; }
        public DateTime DataHora { get; private set; }
        public Pedido Pedido { get; private set; }

        public PedidoStatus(StatusPedido status)
        {
            Status = status;
            DataHora = DateTime.UtcNow;
        }
    }

    public class Pedido : Entity, IAggregateRoot
    {
        public decimal Valor { get; private set; }
        public Guid? ClienteId { get; private set; }
        public ICollection<PedidoItem> Itens { get; private set; }
        public ICollection<PedidoStatus> Status { get; private set; }
        public Guid? PagamentoId { get; private set; }

        protected Pedido() { }

        private Pedido(ICollection<PedidoItem> itens)
        {
            Itens = itens;
            Status = new List<PedidoStatus> {
                new PedidoStatus(StatusPedido.CRIADO)
            };
            Valor = Itens.Sum(i => i.Preco);
            ValidateEntity();
        }

        private Pedido(ICollection<PedidoItem> itens, Guid clienteId) : this(itens)
        {
            ClienteId = clienteId;
            AssertionConcern.AssertArgumentNotEquals(Guid.Empty, ClienteId, "Codigo do cliente não foi informado!");
        }

        public void Pagar(Guid pagamentoId)
        {
            AssertionConcern.AssertArgumentNotNull(PagamentoId, "O código do pagamento não pode ser vazio!");

            if (PagamentoId is null)
            {
                PagamentoId = pagamentoId;
                AtualizarStatus(StatusPedido.RECEBIDO);
            }
        }

        public void IniciarPreparo()
        {
            AssertionConcern.AssertArgumentTrue(ValidarPagamento(), "Não foi realizado o pagamento para o pedido informado!");
            AtualizarStatus(StatusPedido.EM_PREPARACAO);
        }

        public void FinalizarPreparo()
        {
            var preparoPedidoIniciado = VerificarExisteStatus(StatusPedido.EM_PREPARACAO);

            AssertionConcern.AssertArgumentTrue(preparoPedidoIniciado, "Não foi possível finalizar o preparo do pedido pois o preparo não foi iniciado!");

            AtualizarStatus(StatusPedido.PRONTO);
        }

        public void Finalizar()
        {
            var preparoPedidoIniciado = VerificarExisteStatus(StatusPedido.PRONTO);

            AssertionConcern.AssertArgumentTrue(preparoPedidoIniciado, "Não foi possível finalizar o pedido pois o preparo não foi finalizado!");

            AtualizarStatus(StatusPedido.FINALIZADO);
        }

        public StatusPedido RetornarStatusAtual()
        {
            return RetornarUltimoStatus().Status;
        }

        public string RetornarDataHora()
        {
            return RetornarUltimoStatus().DataHora.ToString();
        }

        private PedidoStatus RetornarUltimoStatus()
        {
            return Status.OrderByDescending(p => p.DataHora)
                         .FirstOrDefault();
        }

        private void AtualizarStatus(StatusPedido status)
        {
            AssertionConcern.AssertArgumentNotNull(PagamentoId, "Para avançar com o pedido é necessário realizar o pagamento!");

            if (Status.Any(s => s.Status == status))
                return;

            Status.Add(new PedidoStatus(status));
        }

        private bool ValidarPagamento()
        {
            return PagamentoId != null && PagamentoId != Guid.Empty;
        }

        private bool VerificarExisteStatus(StatusPedido status)
        {
            return Status.Any(s => s.Status == status);
        }

        private void ValidateEntity()
        {
            AssertionConcern.AssertArgumentNotEquals(Itens.Any(), false, "O Pedido deve conter pelo nenos 1 item!");
            AssertionConcern.AssertGratherThanValue(Valor, 0, "O valor do pedido deve ser maior que 0!");
        }

        public static class PedidoFactory
        {
            public static Pedido Criar(ICollection<PedidoItem> itens, Guid? clienteId = null)
            {
                return clienteId is null ? new Pedido(itens) : new Pedido(itens, clienteId.Value);
            }
        }
    }
}