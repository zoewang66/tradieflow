using TradieFlow.Application.DTOs;
using TradieFlow.Domain.Entities;

namespace TradieFlow.Application.Interfaces;

public interface IInvoiceService
{
    Task<InvoiceResponse?> CreateAsync(CreateInvoiceRequest request);  
    Task<IReadOnlyList<InvoiceResponse>> GetByJobAsync(int jobId);
    Task<InvoiceResponse?> GetByIdAsync(int id);
    Task<bool> UpdateStatusAsync(int id, InvoiceStatus status);
    Task<bool> DeleteAsync(int id);
}