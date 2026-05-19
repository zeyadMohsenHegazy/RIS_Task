import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { Subject } from 'rxjs';
import { DashboardStats } from './models/dashboard.model';
import { CacheInvalidationService } from '../../store/cache-invalidation.service';
import {
  AsyncState,
  idleState,
} from '../../store/models/async-state.model';
import { connectAsyncStorePipeline } from '../../store/utils/async-store.pipeline';
import { selectError, selectLoading } from '../../store/utils/store.helpers';
import { DashboardService } from './dashboard.service';

@Injectable({ providedIn: 'root' })
export class DashboardStore {
  private readonly api = inject(DashboardService);
  private readonly cache = inject(CacheInvalidationService);
  private readonly load$ = new Subject<void>();

  private readonly state = signal<AsyncState<DashboardStats>>(idleState());
  private loadedVersion = -1;

  readonly stats = computed(() => this.state().data);
  readonly loading = selectLoading(this.state);
  readonly error = selectError(this.state);
  readonly lowStockProducts = computed(() => this.state().data?.lowStockProducts ?? []);

  constructor() {
    connectAsyncStorePipeline({
      load$: this.load$,
      state: this.state,
      request: () => this.api.getStats(),
      errorMessage:
        'Unable to load dashboard data. Please check your connection and try again.',
      onSuccess: () => {
        this.loadedVersion = this.cache.dashboardVersion();
      },
    });

    effect(() => {
      const version = this.cache.dashboardVersion();
      if (this.loadedVersion >= 0 && version !== this.loadedVersion) {
        this.load(true);
      }
    });
  }

  load(force = false): void {
    const version = this.cache.dashboardVersion();
    if (!force && this.state().status === 'success' && this.loadedVersion === version) {
      return;
    }
    this.load$.next();
  }
}
