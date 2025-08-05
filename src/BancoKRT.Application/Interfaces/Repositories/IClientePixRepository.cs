using BancoKRT.Domain.Entities;

namespace BancoKRT.Application.Interfaces.Repositories
{
    public interface IClientePixRepository
    {
        Task CreateAsync(ClientePix cliente);
        Task<ClientePix> GetByKeyAsync(string documento,
            string contaId);
        Task<bool> UpdateLimitePixAsync(string documento, 
            string contaId, decimal novoLimite);
        Task<bool> DeleteAsync(string documento, 
            string contaId);
        Task<(bool Aprovada, decimal? NovoLimite)> ProcessarTransacaoAsync(string documento,
            string contaId, decimal valorTransacao);
    }
}
