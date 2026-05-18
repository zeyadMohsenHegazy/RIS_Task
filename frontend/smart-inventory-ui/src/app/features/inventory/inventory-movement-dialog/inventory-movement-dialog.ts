import { Component, computed, DestroyRef, effect, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef,
} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { NotificationService } from '../../../core/notifications/notification.service';
import { finalize } from 'rxjs';
import { ProductsStore } from '../../products/products.store';
import {
  InventoryMovementDialogData,
  InventoryMovementDto,
  TRANSACTION_TYPE_FORM_OPTIONS,
  TransactionType,
  getTransactionTypeLabel,
} from '../../../models/inventory.model';
import { CacheInvalidationService } from '../../../store/cache-invalidation.service';
import { InventoryService } from '../inventory.service';
import { FormSkeleton, LoadingButton } from '../../../shared';

@Component({
  selector: 'app-inventory-movement-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    FormSkeleton,
    LoadingButton,
  ],
  templateUrl: './inventory-movement-dialog.html',
  styleUrl: './inventory-movement-dialog.scss',
})
export class InventoryMovementDialog implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly inventoryService = inject(InventoryService);
  private readonly productsStore = inject(ProductsStore);
  private readonly cache = inject(CacheInvalidationService);
  private readonly notifications = inject(NotificationService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly dialogRef = inject(MatDialogRef<InventoryMovementDialog, boolean>);
  readonly data = inject<InventoryMovementDialogData>(MAT_DIALOG_DATA);

  readonly submitting = signal(false);

  readonly typeOptions = TRANSACTION_TYPE_FORM_OPTIONS;
  readonly transactionType = TransactionType;

  readonly products = this.productsStore.pickerProducts;
  readonly productsLoading = this.productsStore.pickerLoading;

  readonly form = this.fb.nonNullable.group({
    productId: [
      this.data.productId ?? 0,
      [Validators.required, Validators.min(1)],
    ],
    transactionType: [
      this.data.transactionType ?? TransactionType.In,
      Validators.required,
    ],
    quantity: [1, [Validators.required, Validators.min(1), this.quantityValidator()]],
  });

  readonly selectedProduct = computed(() => {
    const id = this.form.controls.productId.value;
    return this.products().find((p) => p.id === id) ?? null;
  });

  readonly dialogTitle = computed(() => {
    const type = this.form.controls.transactionType.value;
    return type === TransactionType.Out ? 'Stock Out' : 'Stock In';
  });

  constructor() {
    effect(() => {
      const products = this.products();
      if (products.length === 0) {
        return;
      }
      const preferredId = this.data.productId;
      if (preferredId && products.some((p) => p.id === preferredId)) {
        this.form.patchValue({ productId: preferredId }, { emitEvent: false });
      } else if (this.form.controls.productId.value < 1) {
        this.form.patchValue({ productId: products[0].id }, { emitEvent: false });
      }
    });
  }

  ngOnInit(): void {
    this.productsStore.loadPicker();

    this.form.controls.productId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.form.controls.quantity.updateValueAndValidity());

    this.form.controls.transactionType.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.form.controls.quantity.updateValueAndValidity());
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { productId, transactionType, quantity } = this.form.getRawValue();
    const dto: InventoryMovementDto = { productId, quantity: Number(quantity) };

    this.submitting.set(true);

    const request$ =
      transactionType === TransactionType.In
        ? this.inventoryService.stockIn(dto)
        : this.inventoryService.stockOut(dto);

    request$
      .pipe(
        finalize(() => this.submitting.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (result) => {
          const label = getTransactionTypeLabel(transactionType);
          this.notifications.success(
            `${label}: ${result.quantity} units for ${result.productName}`,
            'Inventory updated',
          );
          this.cache.invalidateInventory();
          this.dialogRef.close(true);
        },
      });
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  get productId() {
    return this.form.controls.productId;
  }

  get transactionTypeControl() {
    return this.form.controls.transactionType;
  }

  get quantity() {
    return this.form.controls.quantity;
  }

  private quantityValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const parent = control.parent;
      if (!parent) {
        return null;
      }

      const type = parent.get('transactionType')?.value as TransactionType;
      const productId = parent.get('productId')?.value as number;
      const qty = Number(control.value);

      if (type !== TransactionType.Out || !productId || Number.isNaN(qty)) {
        return null;
      }

      const product = this.products().find((p) => p.id === productId);
      if (!product) {
        return null;
      }

      if (qty > product.quantity) {
        return {
          insufficientStock: { available: product.quantity, requested: qty },
        };
      }

      return null;
    };
  }
}
