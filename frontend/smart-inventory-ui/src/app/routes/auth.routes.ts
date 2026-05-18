import { Routes } from '@angular/router';
import { guestGuard } from '../guards/guest.guard';

export const AUTH_ROUTES: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('../layout/auth-layout/auth-layout').then((m) => m.AuthLayout),
    children: [
      {
        path: '',
        loadComponent: () =>
          import('../pages/login/login.page').then((m) => m.LoginPage),
      },
    ],
  },
];
