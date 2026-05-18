import { Routes } from '@angular/router';

export const MAIN_ROUTES: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('../pages/dashboard/dashboard.page').then((m) => m.DashboardPage),
    data: { title: 'Dashboard' },
  },
  {
    path: 'products',
    loadComponent: () =>
      import('../pages/products-list/products-list.page').then((m) => m.ProductsListPage),
    data: { title: 'Products' },
  },
  {
    path: 'products/new',
    loadComponent: () =>
      import('../pages/product-form/product-form.page').then((m) => m.ProductFormPage),
    data: { title: 'New product' },
  },
  {
    path: 'products/:id/edit',
    loadComponent: () =>
      import('../pages/product-form/product-form.page').then((m) => m.ProductFormPage),
    data: { title: 'Edit product' },
  },
  {
    path: 'inventory/history',
    loadComponent: () =>
      import('../pages/inventory-history/inventory-history.page').then(
        (m) => m.InventoryHistoryPage,
      ),
    data: { title: 'Inventory history' },
  },
];
