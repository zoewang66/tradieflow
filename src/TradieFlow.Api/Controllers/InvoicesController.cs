using Microsoft.AspNetCore.Mvc;
using TradieFlow.Application.DTOs;
using TradieFlow.Application.Interfaces;

namespace TradieFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    public InvoicesController(IInvoiceService invoiceService) => _invoiceService = invoiceService;

    // GET /api/invoices?jobId=5  → invoices for a job
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InvoiceResponse>>> GetByJob([FromQuery] int jobId)
    {
        var invoices = await _invoiceService.GetByJobAsync(jobId);
        return Ok(invoices);
    }

    // GET /api/invoices/5
    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceResponse>> GetById(int id)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);
        return invoice is null ? NotFound() : Ok(invoice);
    }

    // POST /api/invoices
    [HttpPost]
    public async Task<ActionResult<InvoiceResponse>> Create(CreateInvoiceRequest request)
    {
        var created = await _invoiceService.CreateAsync(request);
        if (created is null)
            return BadRequest($"Job {request.JobId} does not exist.");

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/invoices/5/status   → just change the status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateInvoiceStatusRequest request)
    {
        var updated = await _invoiceService.UpdateStatusAsync(id, request.Status);
        return updated ? NoContent() : NotFound();
    }

    // DELETE /api/invoices/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _invoiceService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}