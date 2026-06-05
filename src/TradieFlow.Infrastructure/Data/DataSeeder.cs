using Microsoft.EntityFrameworkCore;
using TradieFlow.Domain.Entities;

namespace TradieFlow.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(TradieFlowDbContext db)
    {
        // Only seed an empty database. This keeps it idempotent and lets the
        // demo self-heal after a reset or a fresh database — without ever
        // touching data a user has already added.
        if (await db.Clients.AnyAsync())
            return;

        var client = new Client
        {
            Name = "Coastal Renovations",
            CompanyName = "Coastal Renovations Pty Ltd",
            Email = "admin@coastalreno.com.au",
            Phone = "0412 345 678",
            Address = "14 Marine Pde, Hobart TAS 7000",
            Jobs = new List<Job>
            {
                new Job
                {
                    Title = "Master bathroom renovation",
                    Description = "Full strip-out and refit: tiling, plumbing, waterproofing, vanity install.",
                    Status = JobStatus.InProgress,
                    Invoices = new List<Invoice>
                    {
                        new Invoice
                        {
                            Status = InvoiceStatus.Sent,
                            IssuedAt = DateTime.UtcNow,
                            DueAt = DateTime.UtcNow.AddDays(14),
                            LineItems = new List<LineItem>
                            {
                                new LineItem { Description = "Labour (plumbing & install)", Quantity = 24m, UnitPrice = 95.00m },
                                new LineItem { Description = "Wall & floor tiles",          Quantity = 38m, UnitPrice = 42.50m },
                                new LineItem { Description = "Waterproofing & materials",   Quantity = 1m,  UnitPrice = 680.00m },
                            }
                        }
                    }
                }
            }
        };

        db.Clients.Add(client);
        await db.SaveChangesAsync();

        // InvoiceNumber depends on the generated Id — set it after the first save,
        // the same way InvoiceService does (INV-00001, ...).
        var invoice = client.Jobs[0].Invoices[0];
        invoice.InvoiceNumber = $"INV-{invoice.Id:D5}";
        await db.SaveChangesAsync();
    }
}