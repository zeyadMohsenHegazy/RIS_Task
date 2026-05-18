import {
  Component,
  computed,
  DestroyRef,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ToastrService } from 'ngx-toastr';
import {
  debounceTime,
  distinctUntilChanged,
  filter,
  finalize,
  Subject,
  switchMap,
  tap,
} from 'rxjs';
import { AuthService } from '../../features/auth/auth.service';
import { ProductsService } from '../../features/products/products.service';
import { ProductDto } from '../../models/product.model';
import { ConfirmDialogService, ErrorState, LoadingSpinner } from '../../shared';

@Component({
  selector: 'app-products-list-page',
  imports: [
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatTooltipModule,
    ErrorState,
    LoadingSpinner,
  ],
  templateUrl: './products-list.page.html',
  styleUrl: './products-list.page.scss',
})
export class ProductsListPage implements OnInit {
  private readonly productsService = inject(ProductsService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toastr = inject(ToastrService);
  private readonly destroyRef = inject(DestroyRef);

  private readonly load$ = new Subject<void>();

  readonly searchControl = new FormControl('', { nonNullable: true });

  readonly products = signal<ProductDto[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly pageIndex = signal(0);
  readonly pageSize = signal(10);

  readonly isAdmin = this.auth.isAdmin;

  readonly displayedColumns = computed(() => {
    const base = ['name', 'sku', 'price', 'quantity', 'warehouseName'];
    return this.isAdmin() ? [...base, 'actions'] : base;
  });

  readonly pageSizeOptions = [5, 10, 25, 50];

  ngOnInit(): void {
    this.setupProductLoader();
    this.setupSearch();
    this.loadProducts();
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadProducts();
  }

  loadProducts(): void {
    this.load$.next();
  }

  onAddProduct(): void {
    void this.router.navigate(['/products/new']);
  }

  onEditProduct(product: ProductDto): void {
    void this.router.navigate(['/products', product.id, 'edit']);
  }

  onDeleteProduct(product: ProductDto): void {
    this.confirmDialog
      .confirm({
        title: 'Delete product',
        message: `Are you sure you want to delete "${product.name}"? This action cannot be undone.`,
        confirmText: 'Delete',
        cancelText: 'Cancel',
        confirmColor: 'warn',
      })
      .pipe(
        filter((confirmed) => confirmed === true),
        switchMap(() =>
          this.productsService.delete(product.id).pipe(
            tap(() => this.toastr.success(`"${product.name}" was deleted.`)),
          ),
        ),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => this.loadProducts(),
        error: () => this.toastr.error('Failed to delete product.'),
      });
  }

  formatPrice(price: number): string {
    return new Intl.NumberFormat(undefined, {
      style: 'currency',
      currency: 'USD',
    }).format(price);
  }

  private setupProductLoader(): void {
    this.load$
      .pipe(
        tap(() => {
          this.loading.set(true);
          this.error.set(null);
        }),
        switchMap(() =>
          this.productsService
            .getProducts({
              pageNumber: this.pageIndex() + 1,
              pageSize: this.pageSize(),
              search: this.searchControl.value,
            })
            .pipe(finalize(() => this.loading.set(false))),
        ),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (response) => {
          this.products.set(response.items);
          this.totalCount.set(response.totalCount);
        },
        error: () => {
          this.error.set('Unable to load products. Please try again.');
          this.products.set([]);
          this.totalCount.set(0);
        },
      });
  }

  private setupSearch(): void {
    this.searchControl.valueChanges
      .pipe(debounceTime(350), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.pageIndex.set(0);
        this.loadProducts();
      });
  }
}
