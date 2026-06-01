using TradieFlow.Application.DTOs;

namespace TradieFlow.Application.Interfaces;

public interface IClientService
{
    Task<ClientResponse> CreateAsync(CreateClientRequest request);
    Task<IReadOnlyList<ClientResponse>> GetAllAsync();
}