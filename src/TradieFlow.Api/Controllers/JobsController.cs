using Microsoft.AspNetCore.Mvc;
using TradieFlow.Application.DTOs;
using TradieFlow.Application.Interfaces;

namespace TradieFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;
    public JobsController(IJobService jobService) => _jobService = jobService;

    // GET /api/jobs            → all jobs
    // GET /api/jobs?clientId=2 → only that client's jobs
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<JobResponse>>> GetAll([FromQuery] int? clientId)
    {
        var jobs = clientId is null
            ? await _jobService.GetAllAsync()
            : await _jobService.GetByClientAsync(clientId.Value);
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobResponse>> GetById(int id)
    {
        var job = await _jobService.GetByIdAsync(id);
        return job is null ? NotFound() : Ok(job);
    }

    [HttpPost]
    public async Task<ActionResult<JobResponse>> Create(CreateJobRequest request)
    {
        var created = await _jobService.CreateAsync(request);
        if (created is null)
            return BadRequest($"Client {request.ClientId} does not exist.");

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateJobRequest request)
    {
        var updated = await _jobService.UpdateAsync(id, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _jobService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}