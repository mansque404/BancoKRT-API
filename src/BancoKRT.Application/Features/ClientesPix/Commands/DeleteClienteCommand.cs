using BancoKRT.Application.Interfaces.Repositories;
using MediatR;

namespace BancoKRT.Application.Features.ClientesPix.Commands
{
    public record DeleteClienteCommand(string Documento,
        string ContaId) : IRequest<bool>;

    public class DeleteClienteCommandHandler : IRequestHandler<DeleteClienteCommand, bool>
    {
        private readonly IClientePixRepository _repository;

        public DeleteClienteCommandHandler(IClientePixRepository repository) => _repository = repository;

        public async Task<bool> Handle(DeleteClienteCommand request,
            CancellationToken cancellationToken)
        {
            return await _repository.DeleteAsync(request.Documento, request.ContaId);
        }
    }
}
