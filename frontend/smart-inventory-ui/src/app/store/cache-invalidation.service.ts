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

  /** Product catalog or quantities changed (create/update/delete/stock movement). */
  invalidateProducts(): void {
    this.productsVersion.update((v) => v + 1);
    this.dashboardVersion.update((v) => v + 1);
  }

  invalidateWarehouses(): void {
    this.warehousesVersion.update((v) => v + 1);
  }

  invalidateDashboard(): void {
    this.dashboardVersion.update((v) => v + 1);
  }

  /** New inventory transaction recorded. */
  invalidateInventory(): void {
    this.inventoryVersion.update((v) => v + 1);
    this.invalidateProducts();
  }
}
