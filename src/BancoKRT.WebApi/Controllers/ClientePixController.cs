using Asp.Versioning;
using BancoKRT.Application.DTOs;
using BancoKRT.Application.Features.ClientesPix.Commands;
using BancoKRT.Application.Features.ClientesPix.Queries;
using BancoKRT.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BancoKRT.WebApi.Controllers
{
    /// <summary>
    /// Controlador para gerenciar os limites de transações PIX dos clientes.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")] 
    public class ClientePixController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Construtor do ClientesPixController.
        /// </summary>
        /// <param name="mediator">Instância do MediatR para despachar comandos e consultas.</param>
        public ClientePixController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Cadastra um novo cliente com suas informações de conta e limite PIX.
        /// </summary>
        /// <param name="command">Dados necessários para a criação do cliente.</param>
        /// <returns>Retorna o cliente recém-criado.</returns>
        /// <response code="201">Retorna o cliente recém-criado.</response>
        /// <response code="400">Se os dados da requisição forem inválidos.</response>
        /// <response code="409">Se já existir um cliente com o mesmo documento e conta.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ClientePix), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateCliente([FromBody] CreateClienteCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var clienteCriado = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetClienteByKey),
                                   new { version = "1.0", documento = clienteCriado.Documento, contaId = clienteCriado.ContaId },
                                   clienteCriado);
        }

        /// <summary>
        /// Obtém um registro de cliente específico pela combinação de documento e ID da conta.
        /// </summary>
        /// <param name="documento">O número do documento (Chave de Partição).</param>
        /// <param name="contaId">O identificador da conta (Chave de Classificação).</param>
        /// <returns>Os dados do cliente Pix.</returns>
        /// <response code="200">Retorna os dados do cliente encontrado.</response>
        /// <response code="204">Se o cliente não for encontrado.</response>
        [HttpGet("{documento}/{contaId}", Name = "GetClienteByKey")]
        [ProducesResponseType(typeof(ClientePix), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetClienteByKey(string documento, string contaId)
        {
            var query = new GetClienteByKeyQuery(documento, contaId);
            var cliente = await _mediator.Send(query);

            if (cliente == null)
                return NoContent();

            return Ok(cliente);
        }

        /// <summary>
        /// Atualiza o limite PIX de uma conta específica.
        /// </summary>
        /// <param name="documento">Documento do cliente a ser atualizado.</param>
        /// <param name="contaId">ID da conta do cliente a ser atualizada.</param>
        /// <param name="dto">Objeto contendo o novo limite PIX.</param>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     PATCH /api/v1/clientespix/12345678900/1234#56789-0/limite
        ///     {
        ///        "novoLimitePix": 7500.00
        ///     }
        ///
        /// </remarks>
        /// <response code="204">Limite atualizado com sucesso.</response>
        /// <response code="400">Se os dados da requisição forem inválidos.</response>
        /// <response code="404">Se o cliente não for encontrado.</response>
        [HttpPatch("{documento}/{contaId}/limite")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateLimitePix(string documento, string contaId, [FromBody] UpdateLimitePixDto dto)
        {
            var command = new UpdateLimitePixCommand(documento, contaId, dto.NovoLimitePix);
            var sucesso = await _mediator.Send(command);

            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Remove um registro de cliente da base de limites.
        /// </summary>
        /// <param name="documento">Documento do cliente a ser removido.</param>
        /// <param name="contaId">ID da conta do cliente a ser removida.</param>
        /// <response code="204">Registro removido com sucesso.</response>
        /// <response code="404">Se o cliente não for encontrado.</response>
        [HttpDelete("{documento}/{contaId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCliente(string documento, string contaId)
        {
            var command = new DeleteClienteCommand(documento, contaId);
            var sucesso = await _mediator.Send(command);

            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Processa uma transação PIX para uma conta específica.
        /// </summary>
        /// <remarks>
        /// Verifica atomicamente se o limite da conta é suficiente e, em caso afirmativo, debita o valor da transação.
        /// </remarks>
        /// <param name="documento">Documento do cliente que está realizando a transação.</param>
        /// <param name="contaId">ID da conta do cliente.</param>
        /// <param name="dto">Objeto contendo o valor da transação.</param>
        /// <returns>Retorna um objeto indicando se a transação foi aprovada e o novo limite, se aplicável.</returns>
        /// <response code="200">Retorna o resultado da tentativa de transação (aprovada ou negada).</response>
        /// <response code="400">Se os dados da requisição forem inválidos (ex: valor negativo).</response>
        [HttpPost("{documento}/{contaId}/processar-transacao")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProcessarTransacao(string documento, string contaId, [FromBody] ProcessarTransacaoDto dto)
        {
            ProcessarTransacaoCommand command = new(documento, contaId, dto.Valor);
            var resultado = await _mediator.Send(command);

            if (resultado.Aprovada)
                return Ok(new { aprovado = true, mensagem = "Transação PIX aprovada.", novoLimite = resultado.NovoLimite });
            
            return Ok(new { aprovado = false, mensagem = "Transação PIX negada. Limite insuficiente ou cliente não encontrado." });
        }
    }
}