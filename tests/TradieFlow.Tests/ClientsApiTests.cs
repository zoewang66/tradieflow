using System.Net;
using System.Net.Http.Json;
using TradieFlow.Application.DTOs;
using Xunit;

namespace TradieFlow.Tests;

public class ClientsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ClientsApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();   // an HttpClient wired to the in-memory API
    }

    [Fact]
    public async Task Post_ValidClient_ReturnsCreatedWithId()
    {
        var request = new { name = "Integration Co", email = "test@integration.com" };

        var response = await _client.PostAsJsonAsync("/api/clients", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<ClientResponse>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal("Integration Co", created.Name);
    }

    [Fact]
    public async Task Post_MissingName_ReturnsBadRequest()
    {
        var request = new { email = "no-name@test.com" };

        var response = await _client.PostAsJsonAsync("/api/clients", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var response = await _client.GetAsync("/api/clients/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task FullLifecycle_Create_Get_Update_Delete()
    {
        // Create
        var createResp = await _client.PostAsJsonAsync("/api/clients",
            new { name = "Lifecycle Co", email = "life@cycle.com" });
        createResp.EnsureSuccessStatusCode();
        var created = await createResp.Content.ReadFromJsonAsync<ClientResponse>();
        var id = created!.Id;

        // Get by id
        var getResp = await _client.GetAsync($"/api/clients/{id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

        // Update (PUT)
        var updateResp = await _client.PutAsJsonAsync($"/api/clients/{id}",
            new { name = "Lifecycle Updated", email = "updated@cycle.com" });
        Assert.Equal(HttpStatusCode.NoContent, updateResp.StatusCode);

        // Confirm the change stuck
        var afterUpdate = await _client.GetFromJsonAsync<ClientResponse>($"/api/clients/{id}");
        Assert.Equal("Lifecycle Updated", afterUpdate!.Name);

        // Delete, then confirm it's gone
        var deleteResp = await _client.DeleteAsync($"/api/clients/{id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);

        var afterDelete = await _client.GetAsync($"/api/clients/{id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }
}