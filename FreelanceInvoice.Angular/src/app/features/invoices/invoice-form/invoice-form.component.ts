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
    this.invoiceId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.invoiceId) {
      this.isEditMode = true;
      this.loadInvoice();
    } else {
      this.addItem(); // Add an empty item for new invoices
    }
  }

  private loadInvoice(): void {
    this.isLoading = true;
    this.invoiceService.getInvoice(this.invoiceId!).subscribe({
      next: (invoice) => {
        this.invoiceForm.patchValue(invoice);
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
        this.errorMessage = 'Failed to load invoice. Please try again.';
        this.isLoading = false;
        console.error('Error loading invoice:', error);
      }
    });
  }

  get items() {
    return this.invoiceForm.get('items') as FormArray;
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

  calculateItemAmount(item: any): number {
    const quantity = item.get('quantity')?.value || 0;
    const rate = item.get('rate')?.value || 0;
    const tax = item.get('tax')?.value || 0;
    return (quantity * rate) * (1 + tax / 100);
  }

  calculateSubtotal(): number {
    return this.items.controls.reduce((total, item) => {
      return total + this.calculateItemAmount(item as FormGroup);
    }, 0);
  }

  calculateTotal(): number {
    const subtotal = this.calculateSubtotal();
    const taxAmount = this.items.controls.reduce((total, item) => {
      if (item instanceof FormGroup) {
        const itemAmount = this.calculateItemAmount(item);
        const tax = item.get('tax')?.value || 0;
        return total + (itemAmount - itemAmount / (1 + tax / 100));
      }
      return total;
    }, 0);
    return subtotal;
  }

  onSubmit(): void {
    if (this.invoiceForm.invalid) {
      return;
    }

    this.isLoading = true;
    const invoiceData = this.invoiceForm.value;

    // Calculate amounts for each item
    invoiceData.items = invoiceData.items.map((item: any) => ({
      ...item,
      amount: this.calculateItemAmount(this.fb.group(item))
    }));

    // Calculate totals
    invoiceData.subtotal = this.calculateSubtotal();
    invoiceData.tax = this.calculateTotal() - invoiceData.subtotal;
    invoiceData.total = this.calculateTotal();

    if (this.isEditMode) {
      this.invoiceService.updateInvoice(this.invoiceId!, invoiceData).subscribe({
        next: () => {
          this.router.navigate(['/invoices']);
        },
        error: (error) => {
          this.errorMessage = 'Failed to update invoice. Please try again.';
          this.isLoading = false;
          console.error('Error updating invoice:', error);
        }
      });
    } else {
      this.invoiceService.createInvoice(invoiceData).subscribe({
        next: () => {
          this.router.navigate(['/invoices']);
        },
        error: (error) => {
          this.errorMessage = 'Failed to create invoice. Please try again.';
          this.isLoading = false;
          console.error('Error creating invoice:', error);
        }
      });
    }
  }

  get clientId() { return this.invoiceForm.get('clientId'); }
  get clientName() { return this.invoiceForm.get('clientName'); }
  get issueDate() { return this.invoiceForm.get('issueDate'); }
  get dueDate() { return this.invoiceForm.get('dueDate'); }
  get status() { return this.invoiceForm.get('status'); }
  get notes() { return this.invoiceForm.get('notes'); }
  get terms() { return this.invoiceForm.get('terms'); }
} 