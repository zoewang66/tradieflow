using Microsoft.EntityFrameworkCore;
using TradieFlow.Application.DTOs;
using TradieFlow.Application.Interfaces;
using TradieFlow.Domain.Entities;
using TradieFlow.Infrastructure.Data;

namespace TradieFlow.Infrastructure.Services;

public class JobService : IJobService
{
    private readonly TradieFlowDbContext _db;
    public JobService(TradieFlowDbContext db) => _db = db;

    public async Task<JobResponse?> CreateAsync(CreateJobRequest request)
    {
        // make sure the client this job points at actually exists
        var clientExists = await _db.Clients.AnyAsync(c => c.Id == request.ClientId);
        if (!clientExists)
            return null;

        var job = new Job
        {
            ClientId = request.ClientId,
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            ScheduledAt = request.ScheduledAt,
        };
        _db.Jobs.Add(job);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(job.Id);   // reload so ClientName is filled in
    }

    public async Task<IReadOnlyList<JobResponse>> GetAllAsync() =>
        await _db.Jobs.Select(Projection).ToListAsync();

    public async Task<IReadOnlyList<JobResponse>> GetByClientAsync(int clientId) =>
        await _db.Jobs.Where(j => j.ClientId == clientId).Select(Projection).ToListAsync();

    public async Task<JobResponse?> GetByIdAsync(int id) =>
        await _db.Jobs.Where(j => j.Id == id).Select(Projection).FirstOrDefaultAsync();

    public async Task<bool> UpdateAsync(int id, UpdateJobRequest request)
    {
        var job = await _db.Jobs.FindAsync(id);
        if (job is null) return false;

        job.Title = request.Title;
        job.Description = request.Description;
        job.Status = request.Status;
        job.ScheduledAt = request.ScheduledAt;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var job = await _db.Jobs.FindAsync(id);
        if (job is null) return false;

        _db.Jobs.Remove(job);
        await _db.SaveChangesAsync();
        return true;
    }

    // one shared projection from a Job entity → a JobResponse
    private static readonly System.Linq.Expressions.Expression<Func<Job, JobResponse>> Projection =
        j => new JobResponse(
            j.Id, j.ClientId, j.Client.Name, j.Title,
            j.Description, j.Status, j.ScheduledAt, j.CreatedAt);
}