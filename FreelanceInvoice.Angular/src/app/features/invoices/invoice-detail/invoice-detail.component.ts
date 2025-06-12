import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ActivatedRoute, Router } from '@angular/router';
import { InvoiceService } from '../services/invoice.service';
import { Invoice } from '../models/invoice.model';

@Component({
  selector: 'app-invoice-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './invoice-detail.component.html',
  styleUrls: ['./invoice-detail.component.scss']
})
export class InvoiceDetailComponent implements OnInit {
  invoice: Invoice | null = null;
  isLoading = false;
  errorMessage = '';

  constructor(
    private invoiceService: InvoiceService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const invoiceId = Number(this.route.snapshot.params['id']);
    if (invoiceId) {
      this.loadInvoice(invoiceId);
    }
  }

  loadInvoice(id: number): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.invoiceService.getInvoice(id).subscribe({
      next: (invoice) => {
        this.invoice = invoice;
        this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to load invoice. Please try again.';
      }
    });
  }

  markAsPaid(): void {
    if (!this.invoice?.id) return;

    this.isLoading = true;
    this.errorMessage = '';

    this.invoiceService.markAsPaid(this.invoice.id).subscribe({
      next: () => {
        if (this.invoice?.id) {
          this.loadInvoice(this.invoice.id);
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to mark invoice as paid. Please try again.';
      }
    });
  }

  generatePdf(): void {
    if (!this.invoice?.id) return;

    this.isLoading = true;
    this.errorMessage = '';

    this.invoiceService.generatePdf(this.invoice.id).subscribe({
      next: (response) => {
        // Handle PDF download
        const blob = new Blob([response], { type: 'application/pdf' });
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `invoice-${this.invoice!.number}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to generate PDF. Please try again.';
      }
    });
  }

  deleteInvoice(): void {
    if (!this.invoice?.id) return;

    if (confirm('Are you sure you want to delete this invoice?')) {
      this.isLoading = true;
      this.errorMessage = '';

      this.invoiceService.deleteInvoice(this.invoice.id).subscribe({
        next: () => {
          this.router.navigate(['/invoices']);
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = 'Failed to delete invoice. Please try again.';
        }
      });
    }
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'paid':
        return 'bg-green-100 text-green-800';
      case 'pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'overdue':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  }
} 