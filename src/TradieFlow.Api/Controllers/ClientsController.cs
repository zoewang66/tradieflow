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

    // POST /api/clients
    [HttpPost]
    public async Task<ActionResult<ClientResponse>> Create(CreateClientRequest request)
    {
        var created = await _clientService.CreateAsync(request);
        return Created($"/api/clients/{created.Id}", created);
    }
}