import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { finalize, Subject, switchMap, tap } from 'rxjs';
import { PagedResponse } from '../../models/paged-response.model';
import { ProductDto, ProductQueryParams } from '../../models/product.model';
import {
  AsyncState,
  errorState,
  idleState,
  loadingState,
  successState,
} from '../../store/models/async-state.model';
import { CacheInvalidationService } from '../../store/cache-invalidation.service';
import { ProductsService } from './products.service';

export interface ProductsListQuery {
  pageNumber: number;
  pageSize: number;
  search: string;
}

const PICKER_PAGE_SIZE = 100;
const DEFAULT_QUERY: ProductsListQuery = { pageNumber: 1, pageSize: 10, search: '' };

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
  readonly list = computed(() => this.listState());
  readonly products = computed(() => this.listState().data?.items ?? []);
  readonly totalCount = computed(() => this.listState().data?.totalCount ?? 0);
  readonly listLoading = computed(() => this.listState().status === 'loading');
  readonly listError = computed(() => this.listState().error);

  readonly picker = computed(() => this.pickerState());
  readonly pickerProducts = computed(() => this.pickerState().data ?? []);
  readonly pickerLoading = computed(() => this.pickerState().status === 'loading');
  readonly pickerError = computed(() => this.pickerState().error);

  constructor() {
    this.setupListPipeline();
    this.setupPickerPipeline();
    this.setupInvalidationEffects();
  }

  setListQuery(partial: Partial<ProductsListQuery>): void {
    this.query.update((current) => ({ ...current, ...partial }));
    this.loadList();
  }

  loadList(): void {
    this.listLoad$.next();
  }

  /** Cached product list for dropdowns (dialog, etc.). */
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

  private setupListPipeline(): void {
    this.listLoad$
      .pipe(
        tap(() => {
          const previous = this.listState().data;
          this.listState.set(loadingState(previous));
        }),
        switchMap(() => {
          const q = this.query();
          const params: ProductQueryParams = {
            pageNumber: q.pageNumber,
            pageSize: q.pageSize,
            search: q.search || undefined,
          };
          return this.api.getProducts(params).pipe(
            finalize(() => {
              /* status set in subscribe */
            }),
          );
        }),
      )
      .subscribe({
        next: (response) => {
          this.listState.set(successState(response));
          this.listLoadedVersion = this.cache.productsVersion();
        },
        error: () => {
          this.listState.set(
            errorState('Unable to load products. Please try again.', this.listState().data),
          );
        },
      });
  }

  private setupPickerPipeline(): void {
    this.pickerLoad$
      .pipe(
        tap(() => {
          const previous = this.pickerState().data;
          this.pickerState.set(loadingState(previous));
        }),
        switchMap(() =>
          this.api
            .getProducts({ pageNumber: 1, pageSize: PICKER_PAGE_SIZE })
            .pipe(finalize(() => undefined)),
        ),
      )
      .subscribe({
        next: (response) => {
          this.pickerState.set(successState(response.items));
          this.pickerLoadedVersion = this.cache.productsVersion();
        },
        error: () => {
          this.pickerState.set(
            errorState('Failed to load products.', this.pickerState().data),
          );
        },
      });
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
