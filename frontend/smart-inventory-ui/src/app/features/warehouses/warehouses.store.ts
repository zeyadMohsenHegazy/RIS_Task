import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { finalize, Subject, switchMap, tap } from 'rxjs';
import { WarehouseDto } from '../../models/warehouse.model';
import {
  AsyncState,
  errorState,
  idleState,
  loadingState,
  successState,
} from '../../store/models/async-state.model';
import { CacheInvalidationService } from '../../store/cache-invalidation.service';
import { WarehousesService } from './warehouses.service';

@Injectable({ providedIn: 'root' })
export class WarehousesStore {
  private readonly api = inject(WarehousesService);
  private readonly cache = inject(CacheInvalidationService);
  private readonly load$ = new Subject<void>();

  private readonly state = signal<AsyncState<WarehouseDto[]>>(idleState());
  private loadedVersion = -1;

  readonly warehouses = computed(() => this.state().data ?? []);
  readonly loading = computed(() => this.state().status === 'loading');
  readonly error = computed(() => this.state().error);
  readonly isLoaded = computed(() => this.state().status === 'success');

  constructor() {
    this.setupPipeline();
    effect(() => {
      const version = this.cache.warehousesVersion();
      if (this.loadedVersion >= 0 && version !== this.loadedVersion) {
        this.load(true);
      }
    });
  }

  load(force = false): void {
    const version = this.cache.warehousesVersion();
    if (!force && this.state().status === 'success' && this.loadedVersion === version) {
      return;
    }
    this.load$.next();
  }

  private setupPipeline(): void {
    this.load$
      .pipe(
        tap(() => {
          const previous = this.state().data;
          this.state.set(loadingState(previous));
        }),
        switchMap(() => this.api.getAll().pipe(finalize(() => undefined))),
      )
      .subscribe({
        next: (warehouses) => {
          this.state.set(successState(warehouses));
          this.loadedVersion = this.cache.warehousesVersion();
        },
        error: () => {
          this.state.set(
            errorState('Unable to load warehouses. Please try again.', this.state().data),
          );
        },
      });
  }
}
