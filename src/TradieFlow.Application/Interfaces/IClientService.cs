using TradieFlow.Application.DTOs;

namespace TradieFlow.Application.Interfaces;

public interface IClientService
{
    Task<ClientResponse> CreateAsync(CreateClientRequest request);
    Task<IReadOnlyList<ClientResponse>> GetAllAsync();
    Task<ClientResponse?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(int id, UpdateClientRequest request);
    Task<bool> DeleteAsync(int id);
}