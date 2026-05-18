import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { DashboardService } from '../../features/dashboard/dashboard.service';
import {
  DashboardStats,
  LOW_STOCK_THRESHOLD,
  LowStockProduct,
} from '../../features/dashboard/models/dashboard.model';
import {
  DataTable,
  ErrorState,
  LoadingSpinner,
  StatCard,
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
    LoadingSpinner,
  ],
  templateUrl: './dashboard.page.html',
  styleUrl: './dashboard.page.scss',
})
export class DashboardPage implements OnInit {
  private readonly dashboardService = inject(DashboardService);

  readonly stats = signal<DashboardStats | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

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

  readonly lowStockData = computed(() => this.stats()?.lowStockProducts ?? []);

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading.set(true);
    this.error.set(null);

    this.dashboardService.getStats().subscribe({
      next: (data) => {
        this.stats.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load dashboard data. Please check your connection and try again.');
        this.loading.set(false);
        this.stats.set(null);
      },
    });
  }
}
