import { Injectable, signal } from '@angular/core';

/**
 * Central cache versioning — stores watch these signals and reload when they change.
 */
@Injectable({ providedIn: 'root' })
export class CacheInvalidationService {
  readonly productsVersion = signal(0);
  readonly warehousesVersion = signal(0);
  readonly dashboardVersion = signal(0);
  readonly inventoryVersion = signal(0);
  readonly usersVersion = signal(0);

  /** Product catalog changed (create/update/delete). Lists refresh; dashboard waits until opened. */
  invalidateProductCatalog(): void {
    this.productsVersion.update((v) => v + 1);
  }

  /** Dashboard totals / low-stock (expensive aggregate). */
  invalidateDashboardMetrics(): void {
    this.dashboardVersion.update((v) => v + 1);
  }

  /** @deprecated Prefer invalidateProductCatalog */
  invalidateProducts(): void {
    this.invalidateProductCatalog();
  }

  invalidateWarehouses(): void {
    this.warehousesVersion.update((v) => v + 1);
  }

  invalidateDashboard(): void {
    this.invalidateDashboardMetrics();
  }

  /** New inventory transaction — refresh history, product quantities, and dashboard metrics. */
  invalidateInventory(): void {
    this.inventoryVersion.update((v) => v + 1);
    this.invalidateProductCatalog();
    this.invalidateDashboardMetrics();
  }

  invalidateUsers(): void {
    this.usersVersion.update((v) => v + 1);
  }
}
