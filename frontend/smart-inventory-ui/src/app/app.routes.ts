import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { AUTH_ROUTES } from './routes/auth.routes';
import { MAIN_ROUTES } from './routes/main.routes';

export const routes: Routes = [
  ...AUTH_ROUTES,
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./layout/main-layout/main-layout').then((m) => m.MainLayoutComponent),
    children: MAIN_ROUTES,
  },
  { path: '**', redirectTo: 'home' },
];
