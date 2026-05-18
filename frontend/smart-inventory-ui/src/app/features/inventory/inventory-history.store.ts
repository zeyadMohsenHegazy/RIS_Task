import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { Subject } from 'rxjs';
import {
  InventoryHistoryQueryParams,
  InventoryTransactionDto,
  TransactionType,
} from '../../models/inventory.model';
import { PagedResponse } from '../../models/paged-response.model';
import { DEFAULT_PAGE_SIZE } from '../../shared/constants/pagination.constants';
import { CacheInvalidationService } from '../../store/cache-invalidation.service';
import {
  AsyncState,
  idleState,
} from '../../store/models/async-state.model';
import { connectAsyncStorePipeline } from '../../store/utils/async-store.pipeline';
import { selectError, selectLoading } from '../../store/utils/store.helpers';
import { InventoryService } from './inventory.service';

export interface InventoryHistoryQuery {
  pageNumber: number;
  pageSize: number;
  search: string;
  transactionType: TransactionType | null;
}

const DEFAULT_QUERY: InventoryHistoryQuery = {
  pageNumber: 1,
  pageSize: DEFAULT_PAGE_SIZE,
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
  readonly loading = selectLoading(this.state);
  readonly error = selectError(this.state);

  constructor() {
    connectAsyncStorePipeline({
      load$: this.load$,
      state: this.state,
      request: () => {
        const q = this.query();
        const params: InventoryHistoryQueryParams = {
          pageNumber: q.pageNumber,
          pageSize: q.pageSize,
          search: q.search || undefined,
          transactionType: q.transactionType,
        };
        return this.api.getHistory(params);
      },
      errorMessage: 'Unable to load inventory history. Please try again.',
      onSuccess: () => {
        this.loadedVersion = this.cache.inventoryVersion();
      },
    });

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
}
