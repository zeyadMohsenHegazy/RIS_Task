import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { map, Subject } from 'rxjs';
import { PagedResponse } from '../../models/paged-response.model';
import { ProductDto, ProductQueryParams } from '../../models/product.model';
import { DEFAULT_PAGE_SIZE } from '../../shared/constants/pagination.constants';
import { CacheInvalidationService } from '../../store/cache-invalidation.service';
import {
  AsyncState,
  idleState,
} from '../../store/models/async-state.model';
import { connectAsyncStorePipeline } from '../../store/utils/async-store.pipeline';
import { selectError, selectLoading } from '../../store/utils/store.helpers';
import { ProductsService } from './products.service';

export interface ProductsListQuery {
  pageNumber: number;
  pageSize: number;
  search: string;
}

const PICKER_PAGE_SIZE = 100;
const DEFAULT_QUERY: ProductsListQuery = {
  pageNumber: 1,
  pageSize: DEFAULT_PAGE_SIZE,
  search: '',
};

@Injectable({ providedIn: 'root' })
export class ProductsStore {
  private readonly api = inject(ProductsService);
  private readonly cache = inject(CacheInvalidationService);
  private readonly listLoad$ = new Subject<void>();
  private readonly pickerLoad$ = new Subject<void>();

  private readonly listState = signal<AsyncState<PagedResponse<ProductDto>>>(idleState());
  private readonly pickerState = signal<AsyncState<ProductDto[]>>(idleState());
  private readonly query = signal<ProductsListQuery>({ ...DEFAULT_QUERY });
  private listLoadedVersion = -1;
  private pickerLoadedVersion = -1;

  readonly listQuery = this.query.asReadonly();
  readonly products = computed(() => this.listState().data?.items ?? []);
  readonly totalCount = computed(() => this.listState().data?.totalCount ?? 0);
  readonly listLoading = selectLoading(this.listState);
  readonly listError = selectError(this.listState);

  readonly pickerProducts = computed(() => this.pickerState().data ?? []);
  readonly pickerLoading = selectLoading(this.pickerState);
  readonly pickerError = selectError(this.pickerState);

  constructor() {
    connectAsyncStorePipeline({
      load$: this.listLoad$,
      state: this.listState,
      request: () => {
        const q = this.query();
        const params: ProductQueryParams = {
          pageNumber: q.pageNumber,
          pageSize: q.pageSize,
          search: q.search || undefined,
        };
        return this.api.getProducts(params);
      },
      errorMessage: 'Unable to load products. Please try again.',
      onSuccess: () => {
        this.listLoadedVersion = this.cache.productsVersion();
      },
    });

    connectAsyncStorePipeline({
      load$: this.pickerLoad$,
      state: this.pickerState,
      request: () =>
        this.api
          .getProducts({ pageNumber: 1, pageSize: PICKER_PAGE_SIZE })
          .pipe(map((response) => response.items)),
      errorMessage: 'Failed to load products.',
      onSuccess: () => {
        this.pickerLoadedVersion = this.cache.productsVersion();
      },
    });

    this.setupInvalidationEffects();
  }

  setListQuery(partial: Partial<ProductsListQuery>): void {
    this.query.update((current) => ({ ...current, ...partial }));
    this.loadList();
  }

  loadList(): void {
    this.listLoad$.next();
  }

  loadPicker(force = false): void {
    const version = this.cache.productsVersion();
    if (
      !force &&
      this.pickerState().status === 'success' &&
      this.pickerLoadedVersion === version
    ) {
      return;
    }
    this.pickerLoad$.next();
  }

  invalidate(): void {
    this.cache.invalidateProducts();
  }

  private setupInvalidationEffects(): void {
    effect(() => {
      const version = this.cache.productsVersion();
      if (this.listLoadedVersion >= 0 && version !== this.listLoadedVersion) {
        this.loadList();
      }
    });

    effect(() => {
      const version = this.cache.productsVersion();
      if (this.pickerLoadedVersion >= 0 && version !== this.pickerLoadedVersion) {
        this.pickerLoad$.next();
      }
    });
  }
}
