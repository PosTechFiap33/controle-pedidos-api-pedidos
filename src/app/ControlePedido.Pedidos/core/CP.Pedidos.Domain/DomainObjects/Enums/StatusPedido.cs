using System.ComponentModel;

namespace CP.Pedidos.Domain.Enums
{
    public enum StatusPedido
	{
		[Description("Criado")]
		CRIADO = 1,

		[Description("Recebido")]
		RECEBIDO = 2,

		[Description("Em preparacao")]
		EM_PREPARACAO = 3,

        [Description("Pronto")]
        PRONTO = 4,

        [Description("Finalizado")]
		FINALIZADO = 5

	}
}

