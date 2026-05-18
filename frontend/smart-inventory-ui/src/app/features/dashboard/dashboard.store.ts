import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { finalize, Subject, switchMap, tap } from 'rxjs';
import { DashboardStats } from './models/dashboard.model';
import {
  AsyncState,
  errorState,
  idleState,
  loadingState,
  successState,
} from '../../store/models/async-state.model';
import { CacheInvalidationService } from '../../store/cache-invalidation.service';
import { DashboardService } from './dashboard.service';

@Injectable({ providedIn: 'root' })
export class DashboardStore {
  private readonly api = inject(DashboardService);
  private readonly cache = inject(CacheInvalidationService);
  private readonly load$ = new Subject<void>();

  private readonly state = signal<AsyncState<DashboardStats>>(idleState());
  private loadedVersion = -1;

  readonly stats = computed(() => this.state().data);
  readonly loading = computed(() => this.state().status === 'loading');
  readonly error = computed(() => this.state().error);
  readonly lowStockProducts = computed(() => this.state().data?.lowStockProducts ?? []);

  constructor() {
    this.setupPipeline();
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

  private setupPipeline(): void {
    this.load$
      .pipe(
        tap(() => {
          const previous = this.state().data;
          this.state.set(loadingState(previous));
        }),
        switchMap(() => this.api.getStats().pipe(finalize(() => undefined))),
      )
      .subscribe({
        next: (stats) => {
          this.state.set(successState(stats));
          this.loadedVersion = this.cache.dashboardVersion();
        },
        error: () => {
          this.state.set(
            errorState(
              'Unable to load dashboard data. Please check your connection and try again.',
              this.state().data,
            ),
          );
        },
      });
  }
}
