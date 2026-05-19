import { Routes } from '@angular/router';
import { adminGuard } from '../guards/admin.guard';

export const MAIN_ROUTES: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  {
    path: 'home',
    loadComponent: () =>
      import('../pages/home/home.page').then((m) => m.HomePageComponent),
    data: { title: 'Home' },
  },
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
    canActivate: [adminGuard],
    loadComponent: () =>
      import('../pages/product-form/product-form.page').then((m) => m.ProductFormPage),
    data: { title: 'Add product' },
  },
  {
    path: 'products/:id/edit',
    canActivate: [adminGuard],
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
  {
    path: 'warehouses',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('../pages/warehouses-list/warehouses-list.page').then(
        (m) => m.WarehousesListPage,
      ),
    data: { title: 'Warehouses' },
  },
  {
    path: 'users',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('../pages/users-list/users-list.page').then((m) => m.UsersListPage),
    data: { title: 'Users' },
  },
  {
    path: 'users/new',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('../pages/user-form/user-form.page').then((m) => m.UserFormPage),
    data: { title: 'Add user' },
  },
  {
    path: 'users/:id/edit',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('../pages/user-form/user-form.page').then((m) => m.UserFormPage),
    data: { title: 'Edit user' },
  },
];
