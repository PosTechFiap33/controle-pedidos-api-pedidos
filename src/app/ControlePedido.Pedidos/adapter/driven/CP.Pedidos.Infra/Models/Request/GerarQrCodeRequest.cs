using System.Diagnostics.CodeAnalysis;

namespace CP.Pedidos.Infra.Models.Request;

[ExcludeFromCodeCoverage]
public class GerarQrCodeRequest
{
    public IEnumerable<Item> Itens { get; private set; }

    public GerarQrCodeRequest(IEnumerable<Item> itens)
    {
        Itens = itens;
    }
}

[ExcludeFromCodeCoverage]
public class Item {

    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public decimal Preco { get; private set; }
    public int Quantidade { get; private set; }

    public Item(string nome, string descricao, decimal preco, int quantidade)
    {
        Nome = nome;
        Descricao = descricao;
        Preco = preco;
        Quantidade = quantidade;
    }
}
