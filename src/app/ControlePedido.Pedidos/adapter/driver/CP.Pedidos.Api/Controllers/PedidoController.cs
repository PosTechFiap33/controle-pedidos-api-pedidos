using System.Net;
using CP.Pedidos.Application.UseCases.Pedidos;
using CP.Pedidos.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using CP.Pedidos.Domain.Enums;

namespace CP.Pedidos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoController : MainController
{
    public PedidoController(ILogger<PedidoController> logger) : base(logger)
    {
    }

    /// <summary>
    /// Obtém uma lista de pedidos.
    /// </summary>
    /// <remarks>
    /// Retorna todos os pedidos ordenando pelo o status (Pronto > Em preparação > Recebido) do mais antigo para o mais novo 
    /// </remarks>
    /// <param name="useCase">A instância do caso de uso para listar os pedidos.</param>
    /// <returns>Uma lista de pedidos.</returns>
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(ICollection<PedidoDTO>))]
    [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(500, Type = typeof(ValidationProblemDetails))]
    public async Task<ActionResult<ICollection<PedidoDTO>>> Get([FromServices] IListarPedidoUseCase useCase, [FromQuery] StatusPedido? status)
    {
        var result = await useCase.Executar(status);
        return CustomResponse(result);
    }

    /// <summary>
    /// Obtém informações de acompanhamento de um pedido com base no ID fornecido.
    /// </summary>
    /// <remarks>
    /// Retorna os detalhes de acompanhamento do pedido identificado pelo ID. Se o ID não corresponder
    /// a nenhum pedido existente, retorna um erro correspondente.
    /// </remarks>
    /// <param name="useCase">A instância do caso de uso para obter informações de acompanhamento do pedido.</param>
    /// <param name="id">O identificador único do pedido.</param>
    /// <returns>
    /// Uma ação HTTP que retorna as informações de acompanhamento do pedido.
    /// </returns>
    [HttpGet("{id}/acompanhar")]
    [ProducesResponseType(200, Type = typeof(AcompanhamentoPedidoDTO))]
    [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(500, Type = typeof(ValidationProblemDetails))]
    public async Task<ActionResult<AcompanhamentoPedidoDTO>> Acompanhar([FromServices] IAcompanharPedidoUseCase useCase, Guid id)
    {
        var result = await useCase.Executar(id);
        return CustomResponse(result);
    }

    /// <summary>
    /// Cria um novo pedido com base nos dados fornecidos.
    /// </summary>
    /// <remarks>
    /// Cria um novo pedido no sistema com base nos dados fornecidos no corpo da solicitação.
    /// Retorna os detalhes do pedido criado, incluindo seu identificador único (ID), produtos, valor total, e mais.
    /// </remarks>
    /// <param name="useCase">A instância do caso de uso para criar o pedido.</param>
    /// <param name="criarPedido">Os dados do pedido a serem criados.</param>
    /// <returns>Os detalhes do pedido recém-criado.</returns>
    [HttpPost(Name = "PostPedido")]
    [ProducesResponseType(201, Type = typeof(PedidoCriadoDTO))]
    [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(500, Type = typeof(ValidationProblemDetails))]
    public async Task<ActionResult<PedidoCriadoDTO>> Post([FromServices] ICriarPedidoUseCase useCase, [FromBody] CriarPedidoDTO criarPedido)
    {
        var result = await useCase.Executar(criarPedido);
        return CustomResponse(result, HttpStatusCode.Created);
    }

    /// <summary>
    /// Inicia o preparo de um pedido específico.
    /// </summary>
    /// <remarks>
    /// Inicia o processo de preparo de um pedido identificado pelo seu ID.
    /// Retorna um status 200 OK com uma mensagem indicando que o preparo foi iniciado com sucesso.
    /// </remarks>
    /// <param name="pedidoId">O ID do pedido para o qual iniciar o preparo.</param>
    /// <param name="useCase">A instância do caso de uso para iniciar o preparo do pedido.</param>
    /// <returns>Um status 200 OK com uma mensagem indicando que o preparo foi iniciado com sucesso.</returns>
    [HttpPatch("{pedidoId}/iniciar-preparo")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(500, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> IniciarPreparo([FromRoute] Guid pedidoId, [FromServices] IIniciarPreparoPedidoUseCase useCase)
    {
        await useCase.Executar(pedidoId);
        return Ok($"Preparo do pedido {pedidoId} iniciado com sucesso.");
    }

    /// <summary>
    /// Finaliza o preparo de um pedido específico.
    /// </summary>
    /// <remarks>
    /// Conclui o processo de preparo de um pedido identificado pelo seu ID.
    /// Retorna um status 200 OK com uma mensagem indicando que o preparo foi finalizado com sucesso.
    /// </remarks>
    /// <param name="pedidoId">O ID do pedido para o qual finalizar o preparo.</param>
    /// <param name="useCase">A instância do caso de uso para finalizar o preparo do pedido.</param>
    /// <returns>Um status 200 OK com uma mensagem indicando que o preparo foi finalizado com sucesso.</returns>
    [HttpPatch("{pedidoId}/finalizar-preparo")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(500, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> FinalizarPreparo([FromRoute] Guid pedidoId, [FromServices] IFinalizarPreparoPedidoUseCase useCase)
    {
        await useCase.Executar(pedidoId);
        return Ok($"Preparo do pedido {pedidoId} finalizado com sucesso.");
    }


    /// <summary>
    /// Registra a entrega de um pedido específico.
    /// </summary>
    /// <remarks>
    /// Registra a entrega de um pedido identificado pelo seu ID.
    /// Retorna um status 200 OK com uma mensagem indicando que a entrega foi realizada com sucesso.
    /// </remarks>
    /// <param name="pedidoId">O ID do pedido que está sendo entregue.</param>
    /// <param name="useCase">A instância do caso de uso para registrar a entrega do pedido.</param>
    /// <returns>Um status 200 OK com uma mensagem indicando que a entrega foi realizada com sucesso.</returns>
    [HttpPatch("{pedidoId}/entregar")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(500, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Entregar([FromRoute] Guid pedidoId, [FromServices] IEntregarPedidoUseCase useCase)
    {
        await useCase.Executar(pedidoId);
        return Ok($"Entrega do pedido {pedidoId} realizada com sucesso.");
    }

}