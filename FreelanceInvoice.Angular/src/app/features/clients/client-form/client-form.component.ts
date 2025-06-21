import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ClientService } from '../services/client.service';
import { Client } from '../models/client.model';
import { NgxIntlTelInputModule, PhoneNumberFormat, SearchCountryField } from 'ngx-intl-tel-input';

@Component({
  selector: 'app-client-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NgxIntlTelInputModule],
  templateUrl: './client-form.component.html',
  styleUrl: './client-form.component.scss'
})

export class ClientFormComponent implements OnInit {
  clientForm: FormGroup;
  isEditMode = false;
  isLoading = false;
  errorMessage = '';
  clientId: number | null = null;

  // Phone input configuration
  phoneConfig = {
    separateDialCode: true,
    preferredCountries: ['us', 'gb', 'ca', 'au'],
    onlyCountries: [],
    enablePlaceholder: true,
    customPlaceholder: 'Enter phone number',
    numberFormat: PhoneNumberFormat.International,
    searchCountryFlag: true,
    searchCountryField: [SearchCountryField.Iso2, SearchCountryField.Name]
  };

  constructor(
    private fb: FormBuilder,
    private clientService: ClientService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.clientForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      address: this.fb.group({
        street: [''],
        city: [''],
        state: [''],
        zipCode: [''],
        country: ['']
      }),
      taxId: [''],
      notes: ['']
    });
  }

  ngOnInit(): void {
    this.clientId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.clientId) {
      this.isEditMode = true;
      this.loadClient();
    }
  }

  private loadClient(): void {
    this.isLoading = true;
    this.clientService.getClient(this.clientId!).subscribe({
      next: (client) => {
        // Handle phone data for the international phone input
        const formData = { ...client };
        if (formData.phone && typeof formData.phone === 'string') {
          // The phone input expects an object, but we store it as a string
          // We'll let the component handle the conversion
          formData.phone = formData.phone;
        }
        this.clientForm.patchValue(formData);
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load client. Please try again.';
        this.isLoading = false;
        console.error('Error loading client:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.clientForm.invalid) {
      return;
    }

    this.isLoading = true;
    const clientData = this.clientForm.value;
    
    // Handle phone data - extract the full number if it's an object
    if (clientData.phone && typeof clientData.phone === 'object') {
      clientData.phone = clientData.phone.internationalNumber || clientData.phone.number;
    }

    if (this.isEditMode) {
      this.clientService.updateClient(this.clientId!, clientData).subscribe({
        next: () => {
          this.router.navigate(['/clients']);
        },
        error: (error) => {
          this.errorMessage = 'Failed to update client. Please try again.';
          this.isLoading = false;
          console.error('Error updating client:', error);
        }
      });
    } else {
      this.clientService.createClient(clientData).subscribe({
        next: () => {
          this.router.navigate(['/clients']);
        },
        error: (error) => {
          this.errorMessage = 'Failed to create client. Please try again.';
          this.isLoading = false;
          console.error('Error creating client:', error);
        }
      });
    }
  }

  get name() { return this.clientForm.get('name'); }
  get email() { return this.clientForm.get('email'); }
  get phone() { return this.clientForm.get('phone'); }
  get address() { return this.clientForm.get('address') as FormGroup; }
  get taxId() { return this.clientForm.get('taxId'); }
  get notes() { return this.clientForm.get('notes'); }
} 