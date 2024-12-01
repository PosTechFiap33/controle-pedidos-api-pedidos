using System.Threading.Tasks;

namespace CP.Pedidos.Domain.Base
{
    public interface IUnitOfWork
	{
        Task<bool> Commit();
    }
}

