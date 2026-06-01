using Microsoft.EntityFrameworkCore;
using TradieFlow.Application.DTOs;
using TradieFlow.Application.Interfaces;
using TradieFlow.Domain.Entities;
using TradieFlow.Infrastructure.Data;

namespace TradieFlow.Infrastructure.Services;

public class ClientService : IClientService
{
    private readonly TradieFlowDbContext _db;

    public ClientService(TradieFlowDbContext db)
    {
        _db = db;
    }

    public async Task<ClientResponse> CreateAsync(CreateClientRequest request)
    {
        var client = new Client
        {
            Name = request.Name,
            CompanyName = request.CompanyName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            Notes = request.Notes
        };

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();   

        return new ClientResponse(
            client.Id, client.Name, client.CompanyName, client.Email,
            client.Phone, client.Address, client.Notes, client.CreatedAt);
    }

    public async Task<IReadOnlyList<ClientResponse>> GetAllAsync()
    {
        return await _db.Clients
            .OrderByDescending(c => c.CreatedAt)   
            .Select(c => new ClientResponse(
                c.Id, c.Name, c.CompanyName, c.Email,
                c.Phone, c.Address, c.Notes, c.CreatedAt))
            .ToListAsync();
    }

    public async Task<ClientResponse?> GetByIdAsync(int id)
{
    return await _db.Clients
        .Where(c => c.Id == id)
        .Select(c => new ClientResponse(
            c.Id, c.Name, c.CompanyName, c.Email,
            c.Phone, c.Address, c.Notes, c.CreatedAt))
        .FirstOrDefaultAsync();
}

public async Task<bool> UpdateAsync(int id, UpdateClientRequest request)
{
    var client = await _db.Clients.FindAsync(id);
    if (client is null)
        return false;

    client.Name = request.Name;
    client.CompanyName = request.CompanyName;
    client.Email = request.Email;
    client.Phone = request.Phone;
    client.Address = request.Address;
    client.Notes = request.Notes;

    await _db.SaveChangesAsync();
    return true;
}

public async Task<bool> DeleteAsync(int id)
{
    var client = await _db.Clients.FindAsync(id);
    if (client is null)
        return false;

    _db.Clients.Remove(client);
    await _db.SaveChangesAsync();
    return true;
}
}