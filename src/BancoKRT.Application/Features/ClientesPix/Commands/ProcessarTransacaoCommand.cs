using BancoKRT.Application.Interfaces.Repositories;
using MediatR;

namespace BancoKRT.Application.Features.ClientesPix.Commands
{
    public record ProcessarTransacaoCommand(string Documento, 
        string ContaId, decimal Valor) : IRequest<(bool Aprovada, decimal? NovoLimite)>;

    public class ProcessarTransacaoCommandHandler : IRequestHandler<ProcessarTransacaoCommand, (bool Aprovada, decimal? NovoLimite)>
    {
        private readonly IClientePixRepository _repository;

        public ProcessarTransacaoCommandHandler(IClientePixRepository repository) => _repository = repository;

        public async Task<(bool Aprovada, decimal? NovoLimite)> Handle(ProcessarTransacaoCommand request, 
            CancellationToken cancellationToken)
        {
            return await _repository.ProcessarTransacaoAsync(request.Documento, 
                request.ContaId, request.Valor);
        }
    }
}
