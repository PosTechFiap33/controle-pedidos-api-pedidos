using System.Diagnostics.CodeAnalysis;
using CP.Pedidos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlePedido.Infra.Mappings;

[ExcludeFromCodeCoverage]
public class PedidoStatusMapping : IEntityTypeConfiguration<PedidoStatus>
{
    public void Configure(EntityTypeBuilder<PedidoStatus> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.DataHora)
               .IsRequired();

        builder.Property(p => p.Status)
               .IsRequired();

        builder.HasOne(p => p.Pedido)
               .WithMany(p => p.Status)
               .HasForeignKey(p => p.PedidoId);

        builder.ToTable("PedidoStatus");
    }
}

[ExcludeFromCodeCoverage]
public class PedidoItemMapping : IEntityTypeConfiguration<PedidoItem>
{
    public void Configure(EntityTypeBuilder<PedidoItem> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.PedidoId)
               .IsRequired();

        builder.Property(p => p.ProdutoId)
               .IsRequired();
               
        builder.Property(p => p.Nome)
               .IsRequired()
               .HasMaxLength(100);
        
        builder.Ignore(p => p.Descricao);

        builder.Property(p => p.Preco)
               .IsRequired();

        builder.OwnsOne(p => p.Imagem, imagem =>
        {
            imagem.Property(i => i.Nome)
                  .HasColumnName("NomeImagem")
                  .IsRequired(false)
                  .HasMaxLength(100);

            imagem.Property(i => i.Extensao)
                  .HasColumnName("ExtensaoImagem")
                  .IsRequired(false)
                  .HasMaxLength(10);

            imagem.Property(i => i.Url)
                  .HasColumnName("UrlImagem")
                  .IsRequired(false);
        });

        builder.HasOne(p => p.Pedido)
               .WithMany(p => p.Itens)
               .HasForeignKey(p => p.PedidoId);

        builder.ToTable("PedidoItens");
    }
}

public class PedidoMapping : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.PagamentoId)
               .IsRequired(false);

        builder.Property(p => p.Valor)
               .IsRequired();

        builder.Property(p => p.ClienteId)
               .IsRequired(false);

        builder.HasMany(p => p.Itens)
               .WithOne(p => p.Pedido)
               .HasForeignKey(p => p.PedidoId);

        builder.HasMany(p => p.Status)
               .WithOne(p => p.Pedido)
               .HasForeignKey(p => p.PedidoId);

        builder.ToTable("Pedidos");

    }
}
