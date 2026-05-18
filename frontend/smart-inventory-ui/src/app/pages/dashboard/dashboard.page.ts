import { Component, computed, inject, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { DashboardStore } from '../../features/dashboard/dashboard.store';
import { LOW_STOCK_THRESHOLD, LowStockProduct } from '../../features/dashboard/models/dashboard.model';
import {
  DataTable,
  ErrorState,
  StatCard,
  StatCardsSkeleton,
  TableColumn,
} from '../../shared';

@Component({
  selector: 'app-dashboard-page',
  imports: [
    MatCardModule,
    MatChipsModule,
    StatCard,
    DataTable,
    ErrorState,
    StatCardsSkeleton,
  ],
  templateUrl: './dashboard.page.html',
  styleUrl: './dashboard.page.scss',
})
export class DashboardPage implements OnInit {
  private readonly dashboardStore = inject(DashboardStore);

  readonly stats = this.dashboardStore.stats;
  readonly loading = this.dashboardStore.loading;
  readonly error = this.dashboardStore.error;
  readonly lowStockThreshold = LOW_STOCK_THRESHOLD;

  readonly statCards = computed(() => {
    const data = this.stats();
    const isLoading = this.loading();

    return [
      {
        label: 'Total Products',
        value: data?.totalProducts ?? null,
        icon: 'inventory_2',
        accent: 'primary' as const,
        loading: isLoading,
      },
      {
        label: 'Total Warehouses',
        value: data?.totalWarehouses ?? null,
        icon: 'warehouse',
        accent: 'accent' as const,
        loading: isLoading,
      },
      {
        label: 'Inventory Transactions',
        value: data?.totalInventoryTransactions ?? null,
        icon: 'swap_horiz',
        accent: 'neutral' as const,
        loading: isLoading,
      },
      {
        label: 'Low Stock Products',
        value: data?.lowStockCount ?? null,
        icon: 'warning_amber',
        accent: 'warn' as const,
        loading: isLoading,
      },
    ];
  });

  readonly lowStockColumns: TableColumn<LowStockProduct>[] = [
    { key: 'name', label: 'Product', sortable: true },
    { key: 'sku', label: 'SKU', sortable: true },
    { key: 'warehouseName', label: 'Warehouse', sortable: true },
    {
      key: 'quantity',
      label: 'Quantity',
      sortable: true,
      format: (row) => String(row.quantity),
    },
  ];

  readonly lowStockData = this.dashboardStore.lowStockProducts;

  ngOnInit(): void {
    this.dashboardStore.load();
  }

  loadDashboard(): void {
    this.dashboardStore.load(true);
  }
}
