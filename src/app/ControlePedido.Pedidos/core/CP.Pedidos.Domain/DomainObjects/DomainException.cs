using System.Diagnostics.CodeAnalysis;

namespace CP.Pedidos.Domain.Base
{
    [ExcludeFromCodeCoverage]
    public class DomainException : Exception
	{
		public DomainException(string message) : base(message) { }
	}

    [ExcludeFromCodeCoverage]
    public class IntegrationExceptions : Exception
    {
        public IntegrationExceptions(string message) : base(message) { }
    }
}

