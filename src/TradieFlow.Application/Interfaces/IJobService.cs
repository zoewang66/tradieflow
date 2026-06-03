using TradieFlow.Application.DTOs;

namespace TradieFlow.Application.Interfaces;

public interface IJobService
{
    Task<JobResponse?> CreateAsync(CreateJobRequest request);   // null = that client doesn't exist
    Task<IReadOnlyList<JobResponse>> GetAllAsync();
    Task<IReadOnlyList<JobResponse>> GetByClientAsync(int clientId);
    Task<JobResponse?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(int id, UpdateJobRequest request);
    Task<bool> DeleteAsync(int id);
}