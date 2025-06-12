import { Routes } from '@angular/router';
import { authGuard } from '@app/core/guards/auth.guard';
import { HomeComponent } from './components/home/home';
   
export const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then((m) => m.AUTH_ROUTES),
  },
  {
    path: '',
    loadChildren: () => import('./layouts/admin-layout/admin-layout.module').then(m => m.AdminLayoutModule),
    canActivate: [authGuard]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
