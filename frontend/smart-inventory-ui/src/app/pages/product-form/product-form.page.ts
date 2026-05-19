import {
  Component,
  computed,
  DestroyRef,
  effect,
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
import { MatSelectModule } from '@angular/material/select';
import { NotificationService } from '../../core/notifications/notification.service';
import { finalize } from 'rxjs';
import { AuthService } from '../../features/auth/auth.service';
import { ProductsService } from '../../features/products/products.service';
import { ProductsStore } from '../../features/products/products.store';
import { WarehousesStore } from '../../features/warehouses/warehouses.store';
import { ErrorState, FormSkeleton, LoadingButton, PageHeader } from '../../shared';

interface ProductFormPatch {
  name: string;
  sku: string;
  price: number;
  quantity: number;
  warehouseId: number;
}

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
    ErrorState,
    FormSkeleton,
    LoadingButton,
    PageHeader,
  ],
  templateUrl: './product-form.page.html',
  styleUrl: './product-form.page.scss',
})
export class ProductFormPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productsService = inject(ProductsService);
  private readonly productsStore = inject(ProductsStore);
  private readonly warehousesStore = inject(WarehousesStore);
  private readonly auth = inject(AuthService);
  private readonly notifications = inject(NotificationService);
  private readonly destroyRef = inject(DestroyRef);

  private readonly productIdParam = this.route.snapshot.paramMap.get('id');
  readonly productId = this.productIdParam ? Number(this.productIdParam) : null;
  readonly isEditMode = computed(() => this.productId != null && !Number.isNaN(this.productId));

  private readonly pendingPatch = signal<ProductFormPatch | null>(null);

  readonly warehouses = this.warehousesStore.warehouses;
  readonly warehousesLoading = this.warehousesStore.loading;
  readonly productLoading = signal(false);
  readonly submitting = signal(false);
  readonly loadError = signal<string | null>(null);

  readonly pageLoading = computed(
    () => this.warehousesLoading() || this.productLoading(),
  );

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

  constructor() {
    effect(() => {
      if (this.warehousesLoading()) {
        return;
      }

      // Store starts idle (not loading, empty data). Wait until a fetch finishes.
      if (!this.warehousesStore.isLoaded() && !this.warehousesStore.error()) {
        return;
      }

      const warehouseError = this.warehousesStore.error();
      if (warehouseError) {
        this.loadError.set(warehouseError);
        return;
      }

      const warehouses = this.warehouses();
      if (warehouses.length === 0) {
        this.loadError.set('No warehouses available. Create a warehouse before adding products.');
        return;
      }

      this.loadError.set(null);

      const patch = this.pendingPatch();
      if (patch) {
        const warehouseId = warehouses.some((w) => w.id === patch.warehouseId)
          ? patch.warehouseId
          : warehouses[0].id;
        this.form.patchValue({ ...patch, warehouseId });
        this.pendingPatch.set(null);
        return;
      }

      if (!this.isEditMode() && this.form.controls.warehouseId.value < 1) {
        this.form.patchValue({ warehouseId: warehouses[0].id });
      }
    });
  }

  ngOnInit(): void {
    if (!this.auth.isAdmin()) {
      this.notifications.warning('You do not have permission to manage products.');
      void this.router.navigate(['/products']);
      return;
    }

    if (this.isEditMode() && (this.productId == null || Number.isNaN(this.productId))) {
      this.loadError.set('Invalid product id.');
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
          this.notifications.success(
            this.isEditMode() ? 'Product updated successfully.' : 'Product created successfully.',
          );
          void this.router.navigate(['/products']).then(() => {
            this.productsStore.invalidateCatalog();
          });
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

  retryLoad(): void {
    this.loadError.set(null);
    this.loadPageData();
  }

  private loadPageData(): void {
    this.warehousesStore.load(true);

    if (this.isEditMode() && this.productId != null) {
      this.productLoading.set(true);
      this.productsService
        .getById(this.productId)
        .pipe(
          finalize(() => this.productLoading.set(false)),
          takeUntilDestroyed(this.destroyRef),
        )
        .subscribe({
          next: (product) => {
            this.pendingPatch.set({
              name: product.name,
              sku: product.sku,
              price: product.price,
              quantity: product.quantity,
              warehouseId: product.warehouseId,
            });
          },
          error: () => {
            this.loadError.set('Product not found or could not be loaded.');
          },
        });
    }
  }
}
