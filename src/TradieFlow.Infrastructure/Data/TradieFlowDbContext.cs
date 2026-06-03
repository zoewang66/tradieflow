using Microsoft.EntityFrameworkCore;
using TradieFlow.Domain.Entities;

namespace TradieFlow.Infrastructure.Data;

public class TradieFlowDbContext : DbContext
{
    public TradieFlowDbContext(DbContextOptions<TradieFlowDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Invoice> Invoices => Set<Invoice>();       
    public DbSet<LineItem> LineItems => Set<LineItem>();  

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    modelBuilder.Entity<Job>()
        .Property(j => j.Status)
        .HasConversion<string>();

    modelBuilder.Entity<Invoice>()
        .Property(i => i.Status)
        .HasConversion<string>();

    // money: store with 2 decimal places of precision
    modelBuilder.Entity<LineItem>()
        .Property(li => li.Quantity)
        .HasPrecision(18, 2);

    modelBuilder.Entity<LineItem>()
        .Property(li => li.UnitPrice)
        .HasPrecision(18, 2);
    }
}