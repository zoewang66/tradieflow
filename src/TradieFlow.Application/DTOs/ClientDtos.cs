using System.ComponentModel.DataAnnotations;
namespace TradieFlow.Application.DTOs;

public record CreateClientRequest(
    [Required, StringLength(200)] string Name,
    [StringLength(200)] string? CompanyName,
    [EmailAddress, StringLength(256)] string? Email,
    [Phone, StringLength(50)] string? Phone,
    [StringLength(500)] string? Address,
    [StringLength(2000)] string? Notes
);

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
    [Required, StringLength(200)] string Name,
    [StringLength(200)] string? CompanyName,
    [EmailAddress, StringLength(256)] string? Email,
    [Phone, StringLength(50)] string? Phone,
    [StringLength(500)] string? Address,
    [StringLength(2000)] string? Notes
);