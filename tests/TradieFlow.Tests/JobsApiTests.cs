using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TradieFlow.Application.DTOs;
using TradieFlow.Domain.Entities;
using Xunit;

namespace TradieFlow.Tests;

public class JobsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    // the API sends enums as strings ("Scheduled"), so teach the test's JSON reader
    // to understand them too (the default reader would choke on a string enum)
    private static readonly JsonSerializerOptions JsonOpts =
        new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

    public JobsApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // helper: make a client via the API so we have something to hang jobs on
    private async Task<ClientResponse> CreateClientAsync(string name)
    {
        var resp = await _client.PostAsJsonAsync("/api/clients", new { name });
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<ClientResponse>())!;
    }

    [Fact]
    public async Task Post_CreatesJob_AndIncludesClientName()
    {
        var client = await CreateClientAsync("Joiner Co");

        var resp = await _client.PostAsJsonAsync("/api/jobs",
            new { clientId = client.Id, title = "Deck build", status = "Scheduled" });

        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        var job = await resp.Content.ReadFromJsonAsync<JobResponse>(JsonOpts);
        Assert.NotNull(job);
        Assert.Equal("Deck build", job!.Title);
        Assert.Equal("Joiner Co", job.ClientName);          // ← the JOIN worked
        Assert.Equal(JobStatus.Scheduled, job.Status);
    }

    [Fact]
    public async Task Post_NonexistentClient_ReturnsBadRequest()
    {
        var resp = await _client.PostAsJsonAsync("/api/jobs",
            new { clientId = 999999, title = "Ghost job" });

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task GetByClient_ReturnsOnlyThatClientsJobs()
    {
        var clientA = await CreateClientAsync("Client A");
        var clientB = await CreateClientAsync("Client B");
        await _client.PostAsJsonAsync("/api/jobs", new { clientId = clientA.Id, title = "Job A" });
        await _client.PostAsJsonAsync("/api/jobs", new { clientId = clientB.Id, title = "Job B" });

        var jobs = await _client.GetFromJsonAsync<List<JobResponse>>(
            $"/api/jobs?clientId={clientA.Id}", JsonOpts);

        Assert.NotNull(jobs);
        Assert.All(jobs!, j => Assert.Equal(clientA.Id, j.ClientId));   // every result belongs to A
        Assert.Contains(jobs!, j => j.Title == "Job A");
        Assert.DoesNotContain(jobs!, j => j.Title == "Job B");          // B's job is filtered out
    }

    [Fact]
    public async Task FullLifecycle_Create_UpdateStatus_Delete()
    {
        var client = await CreateClientAsync("Lifecycle Client");

        var createResp = await _client.PostAsJsonAsync("/api/jobs",
            new { clientId = client.Id, title = "Reno", status = "Quoted" });
        var job = await createResp.Content.ReadFromJsonAsync<JobResponse>(JsonOpts);
        var id = job!.Id;

        // move it to Completed (PUT replaces the whole job)
        var updateResp = await _client.PutAsJsonAsync($"/api/jobs/{id}",
            new { title = "Reno", status = "Completed" });
        Assert.Equal(HttpStatusCode.NoContent, updateResp.StatusCode);

        var afterUpdate = await _client.GetFromJsonAsync<JobResponse>($"/api/jobs/{id}", JsonOpts);
        Assert.Equal(JobStatus.Completed, afterUpdate!.Status);

        // delete, then confirm it's gone
        var deleteResp = await _client.DeleteAsync($"/api/jobs/{id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);
        var afterDelete = await _client.GetAsync($"/api/jobs/{id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }
}