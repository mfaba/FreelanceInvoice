using FreelanceInvoice.Domain.Entities;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace FreelanceInvoice.Infrastructure.PDF;

public class PdfService : IPdfService
{
    public async Task<byte[]> GenerateInvoicePdfAsync(Invoice invoice)
    {
        using var memoryStream = new MemoryStream();
        var writer = new PdfWriter(memoryStream);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        // Add header
        //var header = new Paragraph($"INVOICE #{invoice.InvoiceNumber}")
        //    .SetTextAlignment(TextAlignment.CENTER)
        //    .SetFontSize(20)
        //    .SetBold();
        //document.Add(header);

        //// Add invoice details
        //document.Add(new Paragraph($"Date: {invoice.IssueDate:MM/dd/yyyy}"));
        //document.Add(new Paragraph($"Due Date: {invoice.DueDate:MM/dd/yyyy}"));
        //document.Add(new Paragraph($"Status: {invoice.Status}"));

        //// Add client information
        //document.Add(new Paragraph("Bill To:"));
        //document.Add(new Paragraph(invoice.Client.Name));
        //document.Add(new Paragraph(invoice.Client.Email));
        //if (!string.IsNullOrEmpty(invoice.Client.Phone))
        //{
        //    document.Add(new Paragraph(invoice.Client.Phone));
        //}
        //if (!string.IsNullOrEmpty(invoice.Client.Address))
        //{
        //    document.Add(new Paragraph(invoice.Client.Address));
        //}

        //// Add items table
        //var table = new Table(4).UseAllAvailableWidth();
        //table.AddHeaderCell("Description");
        //table.AddHeaderCell("Quantity");
        //table.AddHeaderCell("Unit Price");
        //table.AddHeaderCell("Amount");

        //foreach (var item in invoice.Items)
        //{
        //    table.AddCell(item.Description);
        //    table.AddCell(item.Quantity.ToString());
        //    table.AddCell(item.UnitPrice.ToString("C"));
        //    //table.AddCell(item.Amount.ToString("C"));
        //}

        //document.Add(table);

        //// Add totals
        //document.Add(new Paragraph($"Subtotal: {invoice.Subtotal:C}"));
        //if (invoice.TaxAmount > 0)
        //{
        //    document.Add(new Paragraph($"Tax ({invoice.TaxRate:P}): {invoice.TaxAmount:C}"));
        //}
        //document.Add(new Paragraph($"Total: {invoice.Total:C}")
        //    .SetBold());

        //// Add notes if any
        //if (!string.IsNullOrEmpty(invoice.Notes))
        //{
        //    document.Add(new Paragraph("Notes:"));
        //    document.Add(new Paragraph(invoice.Notes));
        //}

        //document.Close();

        return memoryStream.ToArray();
    }
} 