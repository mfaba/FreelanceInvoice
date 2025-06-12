import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ClientService } from '../services/client.service';
import { Client } from '../models/client.model';

@Component({
  selector: 'app-client-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './client-form.component.html'
})
export class ClientFormComponent implements OnInit {
  clientForm: FormGroup;
  isEditMode = false;
  isLoading = false;
  errorMessage = '';
  clientId: number | null = null;

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
        this.clientForm.patchValue(client);
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