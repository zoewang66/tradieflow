using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using TradieFlow.Infrastructure.Data;
using Xunit;

namespace TradieFlow.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // a throwaway PostgreSQL container, just for the tests
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:16")
        .Build();

    // Swap the app's real database for the test container
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // remove the app's existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TradieFlowDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            // point it at the test container instead
            services.AddDbContext<TradieFlowDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));
        });
    }

    // xUnit runs this BEFORE the tests: start the container, create the schema
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TradieFlowDbContext>();
        await db.Database.MigrateAsync();   // runs your real migrations on the fresh DB
    }

    // ...and this AFTER: throw the container away
    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}