using AutoMapper;
using FreelanceInvoice.Application.DTOs;
using FreelanceInvoice.Application.Interfaces;
using FreelanceInvoice.Domain.Common;
using FreelanceInvoice.Domain.Entities;
using FreelanceInvoice.Domain.Events;
using FreelanceInvoice.Domain.Repositories;

namespace FreelanceInvoice.Application.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IUnitOfWork _unitOfWork;

    public ClientService(
        IClientRepository clientRepository,
        IMapper mapper,
        IEventDispatcher eventDispatcher,
        IUnitOfWork unitOfWork)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
        _eventDispatcher = eventDispatcher;
        _unitOfWork = unitOfWork;
    }

    public async Task<ClientDto> GetByIdAsync(Guid id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        if (client == null)
            throw new KeyNotFoundException($"Client with ID {id} not found.");

        return _mapper.Map<ClientDto>(client);
    }

    public async Task<IEnumerable<ClientDto>> GetAllByFreelancerIdAsync(Guid freelancerId)
    {
        var clients = await _clientRepository.GetByFreelancerIdAsync(freelancerId);
        return _mapper.Map<IEnumerable<ClientDto>>(clients);
    }

    public async Task<ClientDto> CreateAsync(Guid freelancerId, CreateClientDto createClientDto)
    {
        if (await _clientRepository.ExistsByEmailAsync(createClientDto.Email, freelancerId))
            throw new InvalidOperationException($"A client with email {createClientDto.Email} already exists.");

        var client = _mapper.Map<CreateClientDto, Client>(createClientDto);
        client.SetFreelancer(freelancerId);

        await _clientRepository.AddAsync(client);
        await _unitOfWork.SaveChangesAsync();
        await _eventDispatcher.DispatchAsync(new ClientCreatedEvent(client));

        return _mapper.Map<ClientDto>(client);
    }

    public async Task<ClientDto> UpdateAsync(Guid id, UpdateClientDto updateClientDto)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        if (client == null)
            throw new KeyNotFoundException($"Client with ID {id} not found.");

        if (await _clientRepository.ExistsByEmailAsync(updateClientDto.Email, client.FreelancerId) &&
            client.Email != updateClientDto.Email)
            throw new InvalidOperationException($"A client with email {updateClientDto.Email} already exists.");

        client.Update(
            updateClientDto.Name,
            updateClientDto.Email,
            updateClientDto.PhoneNumber,
            updateClientDto.CompanyName,
            updateClientDto.Address,
            updateClientDto.TaxNumber
        );

        await _clientRepository.UpdateAsync(client);
        await _unitOfWork.SaveChangesAsync();
        await _eventDispatcher.DispatchAsync(new ClientUpdatedEvent(client));

        return _mapper.Map<ClientDto>(client);
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        if (client == null)
            throw new KeyNotFoundException($"Client with ID {id} not found.");

        client.Deactivate();
        await _clientRepository.UpdateAsync(client);
        await _unitOfWork.SaveChangesAsync();
        await _eventDispatcher.DispatchAsync(new ClientStatusChangedEvent(client, false));

        return true;
    }

    public async Task<bool> ActivateAsync(Guid id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        if (client == null)
            throw new KeyNotFoundException($"Client with ID {id} not found.");

        client.Activate();
        await _clientRepository.UpdateAsync(client);
        await _unitOfWork.SaveChangesAsync();
        await _eventDispatcher.DispatchAsync(new ClientStatusChangedEvent(client, true));

        return true;
    }
}