using FreelanceInvoice.Domain.Common;
using FreelanceInvoice.Domain.Events;
using FreelanceInvoice.Domain.Repositories;
using FreelanceInvoice.Infrastructure.Email;
using FreelanceInvoice.Infrastructure.FileStorage;
using FreelanceInvoice.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelanceInvoice.Infrastructure.Events.Handlers
{
    public class ClientEmailNotificationHandler : IEventHandler<ClientCreatedEvent>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IEmailService _emailService;
        private readonly IFileStorageService _fileStorageService;

        public ClientEmailNotificationHandler(IInvoiceRepository invoiceRepository,
            IClientRepository clientRepository,
            IEmailService emailService,
            IFileStorageService fileStorageService)
        {
            _invoiceRepository = invoiceRepository;
            _clientRepository = clientRepository;
            _emailService = emailService;
            _fileStorageService = fileStorageService;
        }

        public async Task HandleAsync(ClientCreatedEvent @event)
        {
             Console.WriteLine($"ClientCreatedEvent received for Client ID: {@event.Client.Name}");
        }

    }
}
