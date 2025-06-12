import { Routes } from '@angular/router';
import { authGuard } from '@app/core/guards/auth.guard';

export const CLIENT_ROUTES: Routes = [
  {
    path: '',
    canActivate: [authGuard],
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./client-list/client-list.component').then(
            (m) => m.ClientListComponent
          ),
      },
      {
        path: 'new',
        loadComponent: () =>
          import('./client-form/client-form.component').then(
            (m) => m.ClientFormComponent
          ),
      },
      {
        path: ':id/edit',
        loadComponent: () =>
          import('./client-form/client-form.component').then(
            (m) => m.ClientFormComponent
          ),
        data: {
          renderMode: 'client-only'
        }
      },
    ],
  },
]; 