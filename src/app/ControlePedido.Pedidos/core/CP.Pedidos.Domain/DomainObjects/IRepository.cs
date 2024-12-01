using System;

namespace CP.Pedidos.Domain.Base
{
    public interface IRepository<T> : IDisposable where T: IAggregateRoot
	{
		IUnitOfWork UnitOfWork { get; }
	}
}