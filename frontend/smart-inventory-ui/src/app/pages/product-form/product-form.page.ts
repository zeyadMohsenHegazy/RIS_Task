import {
  Component,
  computed,
  DestroyRef,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { ToastrService } from 'ngx-toastr';
import { forkJoin, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { AuthService } from '../../features/auth/auth.service';
import { ProductsService } from '../../features/products/products.service';
import { WarehousesService } from '../../features/warehouses/warehouses.service';
import { WarehouseDto } from '../../models/warehouse.model';
import { ErrorState, LoadingSpinner } from '../../shared';

@Component({
  selector: 'app-product-form-page',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    MatProgressSpinnerModule,
    ErrorState,
    LoadingSpinner,
  ],
  templateUrl: './product-form.page.html',
  styleUrl: './product-form.page.scss',
})
export class ProductFormPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productsService = inject(ProductsService);
  private readonly warehousesService = inject(WarehousesService);
  private readonly auth = inject(AuthService);
  private readonly toastr = inject(ToastrService);
  private readonly destroyRef = inject(DestroyRef);

  private readonly productIdParam = this.route.snapshot.paramMap.get('id');
  readonly productId = this.productIdParam ? Number(this.productIdParam) : null;
  readonly isEditMode = computed(() => this.productId != null && !Number.isNaN(this.productId));

  readonly warehouses = signal<WarehouseDto[]>([]);
  readonly pageLoading = signal(true);
  readonly submitting = signal(false);
  readonly loadError = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    sku: ['', [Validators.required, Validators.maxLength(50)]],
    price: [0, [Validators.required, Validators.min(0.01)]],
    quantity: [0, [Validators.required, Validators.min(0)]],
    warehouseId: [0, [Validators.required, Validators.min(1)]],
  });

  readonly pageTitle = computed(() =>
    this.isEditMode() ? 'Edit Product' : 'Add Product',
  );

  ngOnInit(): void {
    if (!this.auth.isAdmin()) {
      this.toastr.warning('You do not have permission to manage products.');
      void this.router.navigate(['/products']);
      return;
    }

    if (this.isEditMode() && (this.productId == null || Number.isNaN(this.productId))) {
      this.loadError.set('Invalid product id.');
      this.pageLoading.set(false);
      return;
    }

    this.loadPageData();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { name, sku, price, quantity, warehouseId } = this.form.getRawValue();
    const dto = { name, sku, price: Number(price), quantity: Number(quantity), warehouseId };

    this.submitting.set(true);

    const request$ = this.isEditMode()
      ? this.productsService.update(this.productId!, dto)
      : this.productsService.create(dto);

    request$
      .pipe(
        finalize(() => this.submitting.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.toastr.success(
            this.isEditMode() ? 'Product updated successfully.' : 'Product created successfully.',
          );
          void this.router.navigate(['/products']);
        },
        error: (err) => {
          const message =
            err?.error?.message ??
            (this.isEditMode() ? 'Failed to update product.' : 'Failed to create product.');
          this.toastr.error(message);
        },
      });
  }

  get name() {
    return this.form.controls.name;
  }

  get sku() {
    return this.form.controls.sku;
  }

  get price() {
    return this.form.controls.price;
  }

  get quantity() {
    return this.form.controls.quantity;
  }

  get warehouseId() {
    return this.form.controls.warehouseId;
  }

  private loadPageData(): void {
    this.pageLoading.set(true);
    this.loadError.set(null);

    const product$ =
      this.isEditMode() && this.productId != null
        ? this.productsService.getById(this.productId).pipe(
            catchError(() => {
              this.loadError.set('Product not found or could not be loaded.');
              return of(null);
            }),
          )
        : of(null);

    forkJoin({
      warehouses: this.warehousesService.getAll(),
      product: product$,
    })
      .pipe(
        finalize(() => this.pageLoading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: ({ warehouses, product }) => {
          this.warehouses.set(warehouses);

          if (warehouses.length === 0) {
            this.loadError.set('No warehouses available. Create a warehouse before adding products.');
            return;
          }

          if (this.isEditMode() && !product) {
            return;
          }

          if (product) {
            this.form.patchValue({
              name: product.name,
              sku: product.sku,
              price: product.price,
              quantity: product.quantity,
              warehouseId: product.warehouseId,
            });
          } else if (!this.isEditMode()) {
            this.form.patchValue({ warehouseId: warehouses[0].id });
          }
        },
        error: () => {
          this.loadError.set('Unable to load form data. Please try again.');
        },
      });
  }

  retryLoad(): void {
    this.loadPageData();
  }
}
