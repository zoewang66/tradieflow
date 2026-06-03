namespace TradieFlow.Domain.Entities;

public enum InvoiceStatus
{
    Draft,      
    Sent,       
    Paid,       
    Cancelled   
}

public class Invoice
{
    public int Id { get; set; }


    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public string InvoiceNumber { get; set; } = string.Empty;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DueAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public List<LineItem> LineItems { get; set; } = new();
}