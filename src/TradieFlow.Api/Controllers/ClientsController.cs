using Microsoft.AspNetCore.Mvc;
using TradieFlow.Application.DTOs;
using TradieFlow.Application.Interfaces;

namespace TradieFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    // GET /api/clients
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClientResponse>>> GetAll()
    {
        var clients = await _clientService.GetAllAsync();
        return Ok(clients);
    }

    // GET /api/clients/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ClientResponse>> GetById(int id)
    {
        var client = await _clientService.GetByIdAsync(id);
        if (client is null)
            return NotFound();

        return Ok(client);
    }

    // POST /api/clients
    [HttpPost]
    public async Task<ActionResult<ClientResponse>> Create(CreateClientRequest request)
    {
        var created = await _clientService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/clients/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateClientRequest request)
    {
        var updated = await _clientService.UpdateAsync(id, request);
        if (!updated)
            return NotFound();

        return NoContent();
    }

    // DELETE /api/clients/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _clientService.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}