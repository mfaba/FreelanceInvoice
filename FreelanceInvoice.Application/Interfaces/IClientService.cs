using FreelanceInvoice.Application.DTOs;

namespace FreelanceInvoice.Application.Interfaces;

public interface IClientService
{
    Task<ClientDto> GetByIdAsync(Guid id);
    Task<IEnumerable<ClientDto>> GetAllByFreelancerIdAsync(Guid freelancerId);
    Task<ClientDto> CreateAsync(Guid freelancerId, CreateClientDto createClientDto);
    Task<ClientDto> UpdateAsync(Guid id, UpdateClientDto updateClientDto);
    Task<bool> DeactivateAsync(Guid id);
    Task<bool> ActivateAsync(Guid id);
} 