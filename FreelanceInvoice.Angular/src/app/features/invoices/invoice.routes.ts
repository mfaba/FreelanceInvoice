import { Routes } from '@angular/router';
import { authGuard } from '@app/core/guards/auth.guard';

export const INVOICE_ROUTES: Routes = [
  {
    path: '',
    canActivate: [authGuard],
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./invoice-list/invoice-list.component').then((m) => m.InvoiceListComponent),
      },
      {
        path: 'new',
        loadComponent: () =>
          import('./invoice-form/invoice-form.component').then((m) => m.InvoiceFormComponent),
      },
      {
        path: ':id',
        loadComponent: () =>
          import('./invoice-detail/invoice-detail.component').then((m) => m.InvoiceDetailComponent),
        data: {
          renderMode: 'client-only'
        }
      },
      {
        path: ':id/edit',
        loadComponent: () =>
          import('./invoice-form/invoice-form.component').then((m) => m.InvoiceFormComponent),
        data: {
          renderMode: 'client-only'
        }
      },
    ],
  },
]; 