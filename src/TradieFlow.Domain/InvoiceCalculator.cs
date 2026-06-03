using TradieFlow.Domain.Entities;

namespace TradieFlow.Domain;

public record InvoiceTotals(decimal Subtotal, decimal Gst, decimal Total);

public static class InvoiceCalculator
{
    public const decimal GstRate = 0.10m;   // Australian GST is 10%

    public static InvoiceTotals Calculate(IEnumerable<LineItem> lineItems)
    {
        var subtotal = Math.Round(lineItems.Sum(li => li.Quantity * li.UnitPrice), 2);
        var gst = Math.Round(subtotal * GstRate, 2);
        var total = subtotal + gst;
        return new InvoiceTotals(subtotal, gst, total);
    }
}