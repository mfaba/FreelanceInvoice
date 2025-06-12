import { Routes } from '@angular/router';
import { authGuard } from '@app/core/guards/auth.guard';
import { AdminLayoutComponent } from './admin-layout.component';

export const AdminLayoutRoutes: Routes = [
    {
        path: '',
        canActivate: [authGuard],
        component: AdminLayoutComponent,
        children: [
            {
                path: 'dashboard',
                loadComponent: () => import('@app/features/dashboard/dashboard.component').then(m => m.DashboardComponent)
            },
            {
                path: 'invoices',
                loadChildren: () => import('@app/features/invoices/invoice.routes').then(m => m.INVOICE_ROUTES)
            },
            {
                path: 'clients',
                loadChildren: () => import('@app/features/clients/client.routes').then(m => m.CLIENT_ROUTES)
            },
            {
                path: '',
                redirectTo: 'dashboard',
                pathMatch: 'full'
            }
        ]
    }
]; 