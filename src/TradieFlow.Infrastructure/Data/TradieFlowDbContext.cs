using Microsoft.EntityFrameworkCore;
using TradieFlow.Domain.Entities;

namespace TradieFlow.Infrastructure.Data;

public class TradieFlowDbContext : DbContext
{
    public TradieFlowDbContext(DbContextOptions<TradieFlowDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();
}