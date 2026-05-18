import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { guestGuard } from './guards/guest.guard';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./layout/auth-layout/auth-layout').then((m) => m.AuthLayout),
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./pages/login/login.page').then((m) => m.LoginPage),
      },
    ],
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./layout/main-layout/main-layout').then((m) => m.MainLayout),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./pages/dashboard/dashboard.page').then((m) => m.DashboardPage),
      },
      {
        path: 'products',
        loadComponent: () =>
          import('./pages/products-list/products-list.page').then(
            (m) => m.ProductsListPage,
          ),
      },
      {
        path: 'products/new',
        loadComponent: () =>
          import('./pages/product-form/product-form.page').then(
            (m) => m.ProductFormPage,
          ),
      },
      {
        path: 'products/:id/edit',
        loadComponent: () =>
          import('./pages/product-form/product-form.page').then(
            (m) => m.ProductFormPage,
          ),
      },
      {
        path: 'inventory/history',
        loadComponent: () =>
          import('./pages/inventory-history/inventory-history.page').then(
            (m) => m.InventoryHistoryPage,
          ),
      },
    ],
  },
  { path: '**', redirectTo: 'dashboard' },
];
