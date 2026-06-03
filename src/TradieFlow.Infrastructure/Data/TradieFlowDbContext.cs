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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Job>()
        .Property(j => j.Status)
        .HasConversion<string>();   // store "Quoted"/"Completed" instead of 0/4
}
}