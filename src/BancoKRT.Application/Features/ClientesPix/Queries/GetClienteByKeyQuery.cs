using BancoKRT.Application.Interfaces.Repositories;
using BancoKRT.Domain.Entities;
using MediatR;

namespace BancoKRT.Application.Features.ClientesPix.Queries
{
    public record GetClienteByKeyQuery(string Documento, string ContaId) : IRequest<ClientePix>;

    public class GetClienteByKeyQueryHandler : IRequestHandler<GetClienteByKeyQuery, ClientePix>
    {
        private readonly IClientePixRepository _repository;

        public GetClienteByKeyQueryHandler(IClientePixRepository repository) => _repository = repository;

        public async Task<ClientePix> Handle(GetClienteByKeyQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetByKeyAsync(request.Documento, request.ContaId);
        }
    }
}
