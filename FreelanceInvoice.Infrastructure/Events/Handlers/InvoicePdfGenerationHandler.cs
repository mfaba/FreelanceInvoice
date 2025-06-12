using FreelanceInvoice.Domain.Common;
using FreelanceInvoice.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelanceInvoice.Infrastructure.Events.Handlers
{
    internal class InvoicePdfGenerationHandler : IEventHandler<InvoiceCreatedEvent>
    {
        public async Task HandleAsync(InvoiceCreatedEvent @event)
        {
            Console.Write($"{@event.InvoiceId}");
        }
    }
}
