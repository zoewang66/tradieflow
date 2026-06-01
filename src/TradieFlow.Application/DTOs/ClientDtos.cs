namespace TradieFlow.Application.DTOs;

public record CreateClientRequest(string Name,
    string? CompanyName,
    string? Email,
    string? Phone,
    string? Address,
    string? Notes);

public record ClientResponse(
    int Id,
    string Name,
    string? CompanyName,
    string? Email,
    string? Phone,
    string? Address,
    string? Notes,
    DateTime CreatedAt
);

public record UpdateClientRequest(
    string Name, string? CompanyName, string? Email,
    string? Phone, string? Address, string? Notes
);