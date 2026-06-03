using System.ComponentModel.DataAnnotations;
using TradieFlow.Domain.Entities;

namespace TradieFlow.Application.DTOs;

public record CreateJobRequest(
    [Range(1, int.MaxValue, ErrorMessage = "A valid ClientId is required.")] int ClientId,
    [Required, StringLength(200)] string Title,
    [StringLength(2000)] string? Description,
    DateTime? ScheduledAt,
    JobStatus Status = JobStatus.Quoted
);

public record UpdateJobRequest(
    [Required, StringLength(200)] string Title,
    [StringLength(2000)] string? Description,
    JobStatus Status,
    DateTime? ScheduledAt
);

public record JobResponse(
    int Id,
    int ClientId,
    string ClientName,       
    string Title,
    string? Description,
    JobStatus Status,
    DateTime? ScheduledAt,
    DateTime CreatedAt
);