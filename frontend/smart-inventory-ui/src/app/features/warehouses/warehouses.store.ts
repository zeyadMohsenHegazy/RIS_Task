import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { Subject } from 'rxjs';
import { WarehouseDto } from '../../models/warehouse.model';
import { CacheInvalidationService } from '../../store/cache-invalidation.service';
import {
  AsyncState,
  idleState,
} from '../../store/models/async-state.model';
import { connectAsyncStorePipeline } from '../../store/utils/async-store.pipeline';
import { selectError, selectLoading } from '../../store/utils/store.helpers';
import { WarehousesService } from './warehouses.service';

@Injectable({ providedIn: 'root' })
export class WarehousesStore {
  private readonly api = inject(WarehousesService);
  private readonly cache = inject(CacheInvalidationService);
  private readonly load$ = new Subject<void>();

  private readonly state = signal<AsyncState<WarehouseDto[]>>(idleState());
  private loadedVersion = -1;

  readonly warehouses = computed(() => this.state().data ?? []);
  readonly loading = selectLoading(this.state);
  readonly error = selectError(this.state);
  readonly isLoaded = computed(() => this.state().status === 'success');

  constructor() {
    connectAsyncStorePipeline({
      load$: this.load$,
      state: this.state,
      request: () => this.api.getAll(),
      errorMessage: 'Unable to load warehouses. Please try again.',
      onSuccess: () => {
        this.loadedVersion = this.cache.warehousesVersion();
      },
    });

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
}
