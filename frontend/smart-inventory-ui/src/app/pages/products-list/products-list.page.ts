import { Component, computed, DestroyRef, inject, OnInit } from '@angular/core';
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
import { NotificationService } from '../../core/notifications/notification.service';
import { debounceTime, distinctUntilChanged, filter, switchMap, tap } from 'rxjs';
import { AuthService } from '../../features/auth/auth.service';
import { InventoryMovementDialogService } from '../../features/inventory/inventory-movement-dialog.service';
import { ProductsService } from '../../features/products/products.service';
import { ProductsStore } from '../../features/products/products.store';
import { TransactionType } from '../../models/inventory.model';
import { ProductDto } from '../../models/product.model';
import { ConfirmDialogService, ErrorState, TableSkeleton } from '../../shared';

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
    TableSkeleton,
  ],
  templateUrl: './products-list.page.html',
  styleUrl: './products-list.page.scss',
})
export class ProductsListPage implements OnInit {
  private readonly productsStore = inject(ProductsStore);
  private readonly productsService = inject(ProductsService);
  private readonly inventoryDialog = inject(InventoryMovementDialogService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly notifications = inject(NotificationService);
  private readonly destroyRef = inject(DestroyRef);

  readonly searchControl = new FormControl('', { nonNullable: true });

  readonly products = this.productsStore.products;
  readonly totalCount = this.productsStore.totalCount;
  readonly loading = this.productsStore.listLoading;
  readonly error = this.productsStore.listError;
  readonly isAdmin = this.auth.isAdmin;

  readonly pageIndex = computed(
    () => this.productsStore.listQuery().pageNumber - 1,
  );
  readonly pageSize = computed(() => this.productsStore.listQuery().pageSize);

  readonly displayedColumns = computed(() => {
    const base = ['name', 'sku', 'price', 'quantity', 'warehouseName'];
    return this.isAdmin() ? [...base, 'actions'] : base;
  });

  readonly pageSizeOptions = [5, 10, 25, 50];

  ngOnInit(): void {
    this.searchControl.setValue(this.productsStore.listQuery().search, {
      emitEvent: false,
    });
    this.setupSearch();
    this.productsStore.loadList();
  }

  onPageChange(event: PageEvent): void {
    this.productsStore.setListQuery({
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize,
    });
  }

  loadProducts(): void {
    this.productsStore.loadList();
  }

  onAddProduct(): void {
    void this.router.navigate(['/products/new']);
  }

  onStockIn(product?: ProductDto): void {
    this.openInventoryDialog(TransactionType.In, product);
  }

  onStockOut(product?: ProductDto): void {
    this.openInventoryDialog(TransactionType.Out, product);
  }

  private openInventoryDialog(type: TransactionType, product?: ProductDto): void {
    this.inventoryDialog
      .open({
        transactionType: type,
        productId: product?.id,
      })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe();
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
            tap(() => this.notifications.success(`"${product.name}" was deleted.`)),
          ),
        ),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => this.productsStore.invalidate(),
      });
  }

  formatPrice(price: number): string {
    return new Intl.NumberFormat(undefined, {
      style: 'currency',
      currency: 'USD',
    }).format(price);
  }

  private setupSearch(): void {
    this.searchControl.valueChanges
      .pipe(debounceTime(350), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe((search) => {
        this.productsStore.setListQuery({ pageNumber: 1, search });
      });
  }
}
