using Microsoft.EntityFrameworkCore;
using TradieFlow.Application.DTOs;
using TradieFlow.Infrastructure.Data;
using TradieFlow.Infrastructure.Services;

namespace TradieFlow.Tests;

public class ClientServiceTests
{
    // Helper: build a fresh, isolated in-memory database for each test.
    // A unique database name (Guid) means tests never interfere with each other.
    private static TradieFlowDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<TradieFlowDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new TradieFlowDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_SavesClient_AndReturnsItWithId()
    {
        // Arrange — set things up
        using var context = CreateInMemoryContext();
        var service = new ClientService(context);
        var request = new CreateClientRequest("Acme", "Acme Pty Ltd", "hi@acme.com", null, null, null);

        // Act — run the thing we're testing
        var result = await service.CreateAsync(request);

        // Assert — check the outcome
        Assert.NotEqual(0, result.Id);                       // the DB assigned an id
        Assert.Equal("Acme", result.Name);
        Assert.Equal(1, await context.Clients.CountAsync());  // exactly one row saved
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenClientDoesNotExist()
    {
        using var context = CreateInMemoryContext();
        var service = new ClientService(context);

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ChangesFields_WhenClientExists()
    {
        using var context = CreateInMemoryContext();
        var service = new ClientService(context);
        var created = await service.CreateAsync(
            new CreateClientRequest("Old Name", null, null, null, null, null));

        var ok = await service.UpdateAsync(created.Id,
            new UpdateClientRequest("New Name", null, "new@x.com", null, null, null));

        Assert.True(ok);
        var updated = await service.GetByIdAsync(created.Id);
        Assert.Equal("New Name", updated!.Name);
        Assert.Equal("new@x.com", updated.Email);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenClientDoesNotExist()
    {
        using var context = CreateInMemoryContext();
        var service = new ClientService(context);

        var ok = await service.UpdateAsync(999,
            new UpdateClientRequest("X", null, null, null, null, null));

        Assert.False(ok);
    }

    [Fact]
    public async Task DeleteAsync_RemovesClient_WhenItExists()
    {
        using var context = CreateInMemoryContext();
        var service = new ClientService(context);
        var created = await service.CreateAsync(
            new CreateClientRequest("To Delete", null, null, null, null, null));

        var ok = await service.DeleteAsync(created.Id);

        Assert.True(ok);
        Assert.Equal(0, await context.Clients.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenClientDoesNotExist()
    {
        using var context = CreateInMemoryContext();
        var service = new ClientService(context);

        var ok = await service.DeleteAsync(999);

        Assert.False(ok);
    }
}