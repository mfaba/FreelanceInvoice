import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { InvoiceService } from '../services/invoice.service';
import { Invoice, InvoiceItem } from '../models/invoice.model';

@Component({
  selector: 'app-invoice-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './invoice-form.component.html',
  styleUrls: ['./invoice-form.component.scss']
})
export class InvoiceFormComponent implements OnInit {
  invoiceForm: FormGroup;
  isEditMode = false;
  isLoading = false;
  errorMessage = '';
  invoiceId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private invoiceService: InvoiceService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.invoiceForm = this.fb.group({
      number: ['', Validators.required],
      clientId: ['', Validators.required],
      clientName: ['', Validators.required],
      issueDate: ['', Validators.required],
      dueDate: ['', Validators.required],
      status: ['Draft'],
      items: this.fb.array([]),
      notes: [''],
      terms: ['']
    });
  }

  ngOnInit(): void {
    this.invoiceId = this.route.snapshot.params['id'];
    if (this.invoiceId) {
      this.isEditMode = true;
      this.loadInvoice();
    }
  }

  get items() {
    return this.invoiceForm.get('items') as FormArray;
  }

  loadInvoice(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.invoiceService.getInvoice(this.invoiceId!).subscribe({
      next: (invoice) => {
        this.invoiceForm.patchValue({
          number: invoice.number,
          clientId: invoice.clientId,
          clientName: invoice.clientName,
          issueDate: this.formatDate(invoice.issueDate),
          dueDate: this.formatDate(invoice.dueDate),
          status: invoice.status,
          notes: invoice.notes,
          terms: invoice.terms
        });

        // Clear existing items
        while (this.items.length) {
          this.items.removeAt(0);
        }

        // Add invoice items
        invoice.items.forEach(item => {
          this.items.push(this.createItemFormGroup(item));
        });

        this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to load invoice. Please try again.';
      }
    });
  }

  createItemFormGroup(item?: InvoiceItem): FormGroup {
    return this.fb.group({
      description: [item?.description || '', Validators.required],
      quantity: [item?.quantity || 1, [Validators.required, Validators.min(1)]],
      rate: [item?.rate || 0, [Validators.required, Validators.min(0)]],
      tax: [item?.tax || 0, [Validators.required, Validators.min(0)]]
    });
  }

  addItem(): void {
    this.items.push(this.createItemFormGroup());
  }

  removeItem(index: number): void {
    this.items.removeAt(index);
  }

  updateItemAmount(index: number): void {
    const item = this.items.at(index);
    const quantity = item.get('quantity')?.value || 0;
    const rate = item.get('rate')?.value || 0;
    const tax = item.get('tax')?.value || 0;
    const amount = (quantity * rate) + tax;
    item.patchValue({ amount });
  }

  calculateItemAmount(index: number): number {
    const item = this.items.at(index);
    const quantity = item.get('quantity')?.value || 0;
    const rate = item.get('rate')?.value || 0;
    const tax = item.get('tax')?.value || 0;
    return (quantity * rate) + tax;
  }

  calculateTotal(): number {
    return this.items.controls.reduce((total, item) => {
      return total + this.calculateItemAmount(this.items.controls.indexOf(item));
    }, 0);
  }

  formatDate(date: Date | string): string {
    if (!date) return '';
    const d = new Date(date);
    return d.toISOString().split('T')[0];
  }

  onSubmit(): void {
    if (this.invoiceForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const formValue = this.invoiceForm.value;
    const invoice: Invoice = {
      ...formValue,
      subtotal: this.calculateTotal(),
      tax: formValue.items.reduce((sum: number, item: any) => sum + (item.tax || 0), 0),
      total: this.calculateTotal()
    };

    if (this.isEditMode) {
      this.invoiceService.updateInvoice(this.invoiceId!, invoice).subscribe({
        next: () => {
          this.router.navigate(['/invoices']);
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error?.message || 'Failed to update invoice. Please try again.';
        }
      });
    } else {
      this.invoiceService.createInvoice(invoice).subscribe({
        next: () => {
          this.router.navigate(['/invoices']);
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error?.message || 'Failed to create invoice. Please try again.';
        }
      });
    }
  }
} 