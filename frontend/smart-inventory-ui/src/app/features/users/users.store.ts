import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { Subject } from 'rxjs';
import { PagedResponse } from '../../models/paged-response.model';
import { UserDto, UserQueryParams } from '../../models/user.model';
import { DEFAULT_PAGE_SIZE } from '../../shared/constants/pagination.constants';
import { CacheInvalidationService } from '../../store/cache-invalidation.service';
import {
  AsyncState,
  idleState,
} from '../../store/models/async-state.model';
import { connectAsyncStorePipeline } from '../../store/utils/async-store.pipeline';
import { selectError, selectLoading } from '../../store/utils/store.helpers';
import { UsersService } from './users.service';

export interface UsersListQuery {
  pageNumber: number;
  pageSize: number;
  search: string;
}

const DEFAULT_QUERY: UsersListQuery = {
  pageNumber: 1,
  pageSize: DEFAULT_PAGE_SIZE,
  search: '',
};

@Injectable({ providedIn: 'root' })
export class UsersStore {
  private readonly api = inject(UsersService);
  private readonly cache = inject(CacheInvalidationService);
  private readonly listLoad$ = new Subject<void>();

  private readonly listState = signal<AsyncState<PagedResponse<UserDto>>>(idleState());
  private readonly query = signal<UsersListQuery>({ ...DEFAULT_QUERY });
  private listLoadedVersion = -1;

  readonly listQuery = this.query.asReadonly();
  readonly users = computed(() => this.listState().data?.items ?? []);
  readonly totalCount = computed(() => this.listState().data?.totalCount ?? 0);
  readonly listLoading = selectLoading(this.listState);
  readonly listError = selectError(this.listState);

  constructor() {
    connectAsyncStorePipeline({
      load$: this.listLoad$,
      state: this.listState,
      request: () => {
        const q = this.query();
        const params: UserQueryParams = {
          pageNumber: q.pageNumber,
          pageSize: q.pageSize,
          search: q.search || undefined,
        };
        return this.api.getUsers(params);
      },
      errorMessage: 'Unable to load users. Please try again.',
      onSuccess: () => {
        this.listLoadedVersion = this.cache.usersVersion();
      },
    });

    effect(() => {
      const version = this.cache.usersVersion();
      if (this.listLoadedVersion >= 0 && version !== this.listLoadedVersion) {
        this.loadList();
      }
    });
  }

  setListQuery(partial: Partial<UsersListQuery>): void {
    this.query.update((current) => ({ ...current, ...partial }));
    this.loadList();
  }

  loadList(): void {
    this.listLoad$.next();
  }

  ensureListLoaded(): void {
    if (this.listLoading()) {
      return;
    }

    const version = this.cache.usersVersion();
    if (this.listState().status === 'idle' || this.listLoadedVersion !== version) {
      this.loadList();
    }
  }

  invalidateList(): void {
    this.cache.invalidateUsers();
  }
}
