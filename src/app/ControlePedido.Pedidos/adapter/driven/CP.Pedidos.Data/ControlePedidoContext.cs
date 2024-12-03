using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using CP.Pedidos.Domain.Base;
using CP.Pedidos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlePedido.Infra
{
    [ExcludeFromCodeCoverage]
    public class ControlePedidoContext : DbContext, IUnitOfWork
    {
        public ControlePedidoContext(DbContextOptions options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<Pedido> Pedido { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<ValidationResult>();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ControlePedidoContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public async Task<bool> Commit()
        {
            try
            {
                var sucesso = await base.SaveChangesAsync() > 0;
                return sucesso;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}