using System.ComponentModel.DataAnnotations;
using TradieFlow.Domain.Entities;

namespace TradieFlow.Application.DTOs;

public record CreateLineItemRequest(
    [Required, StringLength(500)] string Description,
    [Range(0, double.MaxValue)] decimal Quantity,
    [Range(0, double.MaxValue)] decimal UnitPrice
);

public record CreateInvoiceRequest(
    [Range(1, int.MaxValue)] int JobId,
    DateTime? DueAt,
    string? Notes,
    [MinLength(1, ErrorMessage = "An invoice needs at least one line item.")]
    List<CreateLineItemRequest> LineItems
);

public record UpdateInvoiceStatusRequest(InvoiceStatus Status);

public record LineItemResponse(
    int Id, string Description, decimal Quantity, decimal UnitPrice, decimal LineTotal
);

public record InvoiceResponse(
    int Id,
    int JobId,
    string InvoiceNumber,
    InvoiceStatus Status,
    DateTime IssuedAt,
    DateTime? DueAt,
    string? Notes,
    DateTime CreatedAt,
    IReadOnlyList<LineItemResponse> LineItems,
    decimal Subtotal,
    decimal Gst,
    decimal Total
);