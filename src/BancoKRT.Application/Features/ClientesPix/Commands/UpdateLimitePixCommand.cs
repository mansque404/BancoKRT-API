using BancoKRT.Application.Interfaces.Repositories;
using MediatR;

namespace BancoKRT.Application.Features.ClientesPix.Commands
{
    public record UpdateLimitePixCommand(string Documento, 
        string ContaId, decimal NovoLimite) : IRequest<bool>;

    public class UpdateLimitePixCommandHandler : IRequestHandler<UpdateLimitePixCommand, bool>
    {
        private readonly IClientePixRepository _repository;

        public UpdateLimitePixCommandHandler(IClientePixRepository repository) => _repository = repository;

        public async Task<bool> Handle(UpdateLimitePixCommand request, CancellationToken cancellationToken)
        {
            return await _repository.UpdateLimitePixAsync(request.Documento, request.ContaId, request.NovoLimite);
        }
    }
}
