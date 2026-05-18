import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { finalize, Subject, switchMap, tap } from 'rxjs';
import {
  InventoryHistoryQueryParams,
  InventoryTransactionDto,
  TransactionType,
} from '../../models/inventory.model';
import { PagedResponse } from '../../models/paged-response.model';
import {
  AsyncState,
  errorState,
  idleState,
  loadingState,
  successState,
} from '../../store/models/async-state.model';
import { CacheInvalidationService } from '../../store/cache-invalidation.service';
import { InventoryService } from './inventory.service';

export interface InventoryHistoryQuery {
  pageNumber: number;
  pageSize: number;
  search: string;
  transactionType: TransactionType | null;
}

const DEFAULT_QUERY: InventoryHistoryQuery = {
  pageNumber: 1,
  pageSize: 10,
  search: '',
  transactionType: null,
};

@Injectable({ providedIn: 'root' })
export class InventoryHistoryStore {
  private readonly api = inject(InventoryService);
  private readonly cache = inject(CacheInvalidationService);
  private readonly load$ = new Subject<void>();

  private readonly state = signal<AsyncState<PagedResponse<InventoryTransactionDto>>>(
    idleState(),
  );
  private readonly query = signal<InventoryHistoryQuery>({ ...DEFAULT_QUERY });
  private loadedVersion = -1;

  readonly historyQuery = this.query.asReadonly();
  readonly transactions = computed(() => this.state().data?.items ?? []);
  readonly totalCount = computed(() => this.state().data?.totalCount ?? 0);
  readonly loading = computed(() => this.state().status === 'loading');
  readonly error = computed(() => this.state().error);

  constructor() {
    this.setupPipeline();
    effect(() => {
      const version = this.cache.inventoryVersion();
      if (this.loadedVersion >= 0 && version !== this.loadedVersion) {
        this.load();
      }
    });
  }

  setQuery(partial: Partial<InventoryHistoryQuery>): void {
    this.query.update((current) => ({ ...current, ...partial }));
    this.load();
  }

  load(): void {
    this.load$.next();
  }

  invalidate(): void {
    this.cache.invalidateInventory();
  }

  private setupPipeline(): void {
    this.load$
      .pipe(
        tap(() => {
          const previous = this.state().data;
          this.state.set(loadingState(previous));
        }),
        switchMap(() => {
          const q = this.query();
          const params: InventoryHistoryQueryParams = {
            pageNumber: q.pageNumber,
            pageSize: q.pageSize,
            search: q.search || undefined,
            transactionType: q.transactionType,
          };
          return this.api.getHistory(params).pipe(finalize(() => undefined));
        }),
      )
      .subscribe({
        next: (response) => {
          this.state.set(successState(response));
          this.loadedVersion = this.cache.inventoryVersion();
        },
        error: () => {
          this.state.set(
            errorState(
              'Unable to load inventory history. Please try again.',
              this.state().data,
            ),
          );
        },
      });
  }
}
