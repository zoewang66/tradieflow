using TradieFlow.Domain;
using TradieFlow.Domain.Entities;
using Xunit;

namespace TradieFlow.Tests;

public class InvoiceCalculatorTests
{
    [Fact]
    public void Calculate_SumsLineItems_AndAdds10PercentGst()
    {
        var lineItems = new List<LineItem>
        {
            new() { Description = "Labour",    Quantity = 8, UnitPrice = 90 },   // 720
            new() { Description = "Materials", Quantity = 1, UnitPrice = 250 },  // 250
        };

        var totals = InvoiceCalculator.Calculate(lineItems);

        Assert.Equal(970m, totals.Subtotal);   // 720 + 250
        Assert.Equal(97m, totals.Gst);         // 10% of 970
        Assert.Equal(1067m, totals.Total);     // 970 + 97
    }

    [Fact]
    public void Calculate_RoundsGstToTwoDecimals()
    {
        var lineItems = new List<LineItem>
        {
            new() { Description = "Odd", Quantity = 1, UnitPrice = 99.99m },
        };

        var totals = InvoiceCalculator.Calculate(lineItems);

        Assert.Equal(99.99m, totals.Subtotal);
        Assert.Equal(10.00m, totals.Gst);      // 9.999 rounds to 10.00
        Assert.Equal(109.99m, totals.Total);
    }

    [Fact]
    public void Calculate_EmptyInvoice_IsAllZero()
    {
        var totals = InvoiceCalculator.Calculate(new List<LineItem>());

        Assert.Equal(0m, totals.Subtotal);
        Assert.Equal(0m, totals.Gst);
        Assert.Equal(0m, totals.Total);
    }
}