using BancoKRT.Application.Interfaces.Repositories;
using BancoKRT.Domain.Entities;
using MediatR;

namespace BancoKRT.Application.Features.ClientesPix.Commands
{
    public record CreateClienteCommand(
          string Documento,
          string ContaId,
          decimal LimitePix,
          string NumeroAgencia,
          string NumeroConta) : IRequest<ClientePix>;

    public class CreateClienteCommandHandler : IRequestHandler<CreateClienteCommand, ClientePix>
    {
        private readonly IClientePixRepository _repository;

        public CreateClienteCommandHandler(IClientePixRepository repository) => _repository = repository;

        public async Task<ClientePix> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
        {
            var cliente = new ClientePix
            {
                Documento = request.Documento,
                ContaId = request.ContaId,
                LimitePix = request.LimitePix,
                NumeroAgencia = request.NumeroAgencia,
                NumeroConta = request.NumeroConta
            };

            await _repository.CreateAsync(cliente);

            return cliente;
        }
    }
}
