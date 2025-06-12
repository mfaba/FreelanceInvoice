using AutoMapper;
using FreelanceInvoice.Application.DTOs;
using FreelanceInvoice.Domain.Entities;

namespace FreelanceInvoice.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Client mappings
        CreateMap<Client, ClientDto>();
        CreateMap<CreateClientDto, Client>();
        CreateMap<UpdateClientDto, Client>();

        // Invoice mappings
        //CreateMap<Invoice, InvoiceDto>();
        //CreateMap<CreateInvoiceDto, Invoice>();
        //CreateMap<UpdateInvoiceDto, Invoice>();

        //// InvoiceItem mappings
        //CreateMap<InvoiceItem, InvoiceItemDto>();
        //CreateMap<CreateInvoiceItemDto, InvoiceItem>();
        //CreateMap<UpdateInvoiceItemDto, InvoiceItem>();

        //// User mappings
        //CreateMap<User, UserDto>();
        //CreateMap<CreateUserDto, User>();
        //CreateMap<UpdateUserDto, User>();
    }
} 