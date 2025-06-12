using FreelanceInvoice.Application.DTOs;
using FreelanceInvoice.Application.Interfaces;
using FreelanceInvoice.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreelanceInvoice.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Freelancer")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly IUserRepository _userRepository;

    public ClientsController(IClientService clientService, IUserRepository userRepository)
    {
        _clientService = clientService;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetAll()
    {
        var freelancerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var clients = await _clientService.GetAllByFreelancerIdAsync(freelancerId);
        return Ok(clients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> GetById(Guid id)
    {
        var client = await _clientService.GetByIdAsync(id);
        return Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create(CreateClientDto createClientDto)
    {
        var identityUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var freelancer = await _userRepository.GetByIdentityIdAsync(identityUserId.ToString());
        var client = await _clientService.CreateAsync(freelancer.Id, createClientDto);
        return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClientDto>> Update(Guid id, UpdateClientDto updateClientDto)
    {
        var client = await _clientService.UpdateAsync(id, updateClientDto);
        return Ok(client);
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _clientService.DeactivateAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _clientService.ActivateAsync(id);
        return NoContent();
    }
} 