using BancoKRT.Application.DTOs;
using BancoKRT.Application.Features.ClientesPix.Commands;
using BancoKRT.Application.Features.ClientesPix.Queries;
using BancoKRT.Domain.Entities;
using BancoKRT.WebApi.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BancoKRT.Api.Tests
{
    public class ClientePixControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly ClientePixController _controller;

        public ClientePixControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new ClientePixController(_mockMediator.Object);
        }

        [Fact]
        public async Task GetClienteByKey_ComChaveExistente_DeveRetornarOkComCliente()
        {
            // Arrange
            var clienteFake = new ClientePix { Documento = "123", ContaId = "456" };
            _mockMediator.Setup(m => m.Send(It.IsAny<GetClienteByKeyQuery>(), default))
                         .ReturnsAsync(clienteFake);

            // Act
            var result = await _controller.GetClienteByKey("123", "456");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(clienteFake, okResult.Value);
        }

        [Fact]
        public async Task GetClienteByKey_ComChaveInexistente_DeveRetornarNotFound()
        {
            // Arrange
            _mockMediator.Setup(m => m.Send(It.IsAny<GetClienteByKeyQuery>(), default))
                         .ReturnsAsync((ClientePix)null);

            // Act
            var result = await _controller.GetClienteByKey("999", "000");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CreateCliente_ComComandoValido_DeveRetornarCreatedAtAction()
        {
            // Arrange
            var command = new CreateClienteCommand("123", "456", 1000, "001", "12345");
            var clienteCriado = new ClientePix { Documento = "123", ContaId = "456" };
            _mockMediator.Setup(m => m.Send(command, default)).ReturnsAsync(clienteCriado);

            // Act
            var result = await _controller.CreateCliente(command);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(ClientePixController.GetClienteByKey), createdAtActionResult.ActionName);
            Assert.Equal(clienteCriado, createdAtActionResult.Value);
        }

        [Fact]
        public async Task UpdateLimitePix_ComSucesso_DeveRetornarNoContent()
        {
            // Arrange
            var dto = new UpdateLimitePixDto { NovoLimitePix = 5000 };
            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLimitePixCommand>(), default))
                         .ReturnsAsync(true); // Simula que a atualização foi bem-sucedida

            // Act
            var result = await _controller.UpdateLimitePix("123", "456", dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateLimitePix_QuandoClienteNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var dto = new UpdateLimitePixDto { NovoLimitePix = 5000 };
            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLimitePixCommand>(), default))
                         .ReturnsAsync(false); // Simula que a atualização falhou (cliente não encontrado)

            // Act
            var result = await _controller.UpdateLimitePix("999", "000", dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteCliente_ComSucesso_DeveRetornarNoContent()
        {
            // Arrange
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteClienteCommand>(), default))
                         .ReturnsAsync(true); // Simula que a deleção foi bem-sucedida

            // Act
            var result = await _controller.DeleteCliente("123", "456");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCliente_QuandoClienteNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteClienteCommand>(), default))
                         .ReturnsAsync(false); // Simula que a deleção falhou (cliente não encontrado)

            // Act
            var result = await _controller.DeleteCliente("999", "000");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ProcessarTransacao_QuandoAprovada_DeveRetornarOkComStatusAprovado()
        {
            // Arrange
            var dto = new ProcessarTransacaoDto { Valor = 100 };
            (bool Aprovada, decimal? NovoLimite) resultadoSimulado = (true, 4900m);
            _mockMediator.Setup(m => m.Send(It.IsAny<ProcessarTransacaoCommand>(), default))
                         .ReturnsAsync(resultadoSimulado);

            // Act
            var result = await _controller.ProcessarTransacao("123", "456", dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value; // Usamos dynamic para acessar as propriedades do objeto anônimo
            Assert.True(value.aprovado);
            Assert.Equal(4900m, value.novoLimite);
        }

        [Fact]
        public async Task ProcessarTransacao_QuandoNegada_DeveRetornarOkComStatusNegado()
        {
            // Arrange
            var dto = new ProcessarTransacaoDto { Valor = 9000 };
            (bool Aprovada, decimal? NovoLimite) resultadoSimulado = (false, null);
            _mockMediator.Setup(m => m.Send(It.IsAny<ProcessarTransacaoCommand>(), default))
                         .ReturnsAsync(resultadoSimulado);

            // Act
            var result = await _controller.ProcessarTransacao("123", "456", dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.False(value.aprovado);
        }

    }
}