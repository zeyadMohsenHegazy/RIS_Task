import { Component, computed, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../features/auth/auth.service';
import { DashboardStore } from '../../features/dashboard/dashboard.store';
import { BRAND } from '../../core/brand.config';
import { ErrorState, StatCard, StatCardsSkeleton } from '../../shared';

interface QuickLink {
  label: string;
  description: string;
  icon: string;
  route: string;
  adminOnly?: boolean;
}

@Component({
  selector: 'app-home-page',
  imports: [
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    StatCard,
    StatCardsSkeleton,
    ErrorState,
  ],
  templateUrl: './home.page.html',
  styleUrl: './home.page.scss',
})
export class HomePageComponent implements OnInit {
  private readonly dashboardStore = inject(DashboardStore);
  readonly auth = inject(AuthService);
  readonly brand = BRAND;

  readonly stats = this.dashboardStore.stats;
  readonly loading = this.dashboardStore.loading;
  readonly error = this.dashboardStore.error;

  readonly greeting = computed(() => {
    const hour = new Date().getHours();
    if (hour < 12) {
      return 'Good morning';
    }
    if (hour < 18) {
      return 'Good afternoon';
    }
    return 'Good evening';
  });

  readonly summaryCards = computed(() => {
    const data = this.stats();
    return [
      {
        label: 'Products',
        value: data?.totalProducts ?? null,
        icon: 'inventory_2',
        accent: 'primary' as const,
      },
      {
        label: 'Warehouses',
        value: data?.totalWarehouses ?? null,
        icon: 'warehouse',
        accent: 'accent' as const,
      },
      {
        label: 'Transactions',
        value: data?.totalInventoryTransactions ?? null,
        icon: 'swap_horiz',
        accent: 'neutral' as const,
      },
      {
        label: 'Low stock',
        value: data?.lowStockCount ?? null,
        icon: 'warning_amber',
        accent: 'warn' as const,
      },
    ];
  });

  readonly quickLinks = computed<QuickLink[]>(() => {
    const links: QuickLink[] = [
      {
        label: 'Dashboard',
        description: 'Full metrics and low-stock report',
        icon: 'dashboard',
        route: '/dashboard',
      },
      {
        label: 'Products',
        description: 'Browse and manage product catalog',
        icon: 'inventory_2',
        route: '/products',
      },
      {
        label: 'Inventory history',
        description: 'Stock in/out transaction log',
        icon: 'history',
        route: '/inventory/history',
      },
    ];

    if (this.auth.isAdmin()) {
      links.push(
        {
          label: 'Add product',
          description: 'Create a new product entry',
          icon: 'add_box',
          route: '/products/new',
          adminOnly: true,
        },
        {
          label: 'Warehouses',
          description: 'View warehouse locations',
          icon: 'warehouse',
          route: '/warehouses',
          adminOnly: true,
        },
      );
    }

    return links;
  });

  ngOnInit(): void {
    this.dashboardStore.load();
  }

  reload(): void {
    this.dashboardStore.load(true);
  }
}
