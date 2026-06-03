using Microsoft.EntityFrameworkCore;
using TradieFlow.Application.DTOs;
using TradieFlow.Application.Interfaces;
using TradieFlow.Domain;
using TradieFlow.Domain.Entities;
using TradieFlow.Infrastructure.Data;

namespace TradieFlow.Infrastructure.Services;

public class InvoiceService : IInvoiceService
{
    private readonly TradieFlowDbContext _db;
    public InvoiceService(TradieFlowDbContext db) => _db = db;

    public async Task<InvoiceResponse?> CreateAsync(CreateInvoiceRequest request)
    {
        var jobExists = await _db.Jobs.AnyAsync(j => j.Id == request.JobId);
        if (!jobExists) return null;

        var invoice = new Invoice
        {
            JobId = request.JobId,
            DueAt = request.DueAt,
            Notes = request.Notes,
            Status = InvoiceStatus.Draft,
            LineItems = request.LineItems
                .Select(li => new LineItem
                {
                    Description = li.Description,
                    Quantity = li.Quantity,
                    UnitPrice = li.UnitPrice,
                })
                .ToList(),
        };

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();                    // inserts the invoice AND its line items, assigns the Id

        invoice.InvoiceNumber = $"INV-{invoice.Id:D5}";  // e.g. INV-00001
        await _db.SaveChangesAsync();

        return ToResponse(invoice);
    }

    public async Task<IReadOnlyList<InvoiceResponse>> GetByJobAsync(int jobId)
    {
        var invoices = await _db.Invoices
            .Include(i => i.LineItems)
            .Where(i => i.JobId == jobId)
            .ToListAsync();

        return invoices.Select(ToResponse).ToList();
    }

    public async Task<InvoiceResponse?> GetByIdAsync(int id)
    {
        var invoice = await _db.Invoices
            .Include(i => i.LineItems)
            .FirstOrDefaultAsync(i => i.Id == id);

        return invoice is null ? null : ToResponse(invoice);
    }

    public async Task<bool> UpdateStatusAsync(int id, InvoiceStatus status)
    {
        var invoice = await _db.Invoices.FindAsync(id);
        if (invoice is null) return false;

        invoice.Status = status;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var invoice = await _db.Invoices.FindAsync(id);
        if (invoice is null) return false;

        _db.Invoices.Remove(invoice);   // cascade also deletes its line items
        await _db.SaveChangesAsync();
        return true;
    }

    // map an Invoice (with its line items loaded) → an InvoiceResponse, computing the totals
    private static InvoiceResponse ToResponse(Invoice invoice)
    {
        var totals = InvoiceCalculator.Calculate(invoice.LineItems);

        var lines = invoice.LineItems
            .Select(li => new LineItemResponse(
                li.Id, li.Description, li.Quantity, li.UnitPrice, li.Quantity * li.UnitPrice))
            .ToList();

        return new InvoiceResponse(
            invoice.Id, invoice.JobId, invoice.InvoiceNumber, invoice.Status,
            invoice.IssuedAt, invoice.DueAt, invoice.Notes, invoice.CreatedAt,
            lines, totals.Subtotal, totals.Gst, totals.Total);
    }
}