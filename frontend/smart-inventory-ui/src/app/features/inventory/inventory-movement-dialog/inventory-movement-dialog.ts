import { Component, computed, inject, OnInit, signal } from '@angular/core';
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
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs';
import { ProductsService } from '../../products/products.service';
import { ProductDto } from '../../../models/product.model';
import {
  InventoryMovementDialogData,
  InventoryMovementDto,
  TRANSACTION_TYPE_FORM_OPTIONS,
  TransactionType,
  getTransactionTypeLabel,
} from '../../../models/inventory.model';
import { InventoryService } from '../inventory.service';
import { LoadingSpinner } from '../../../shared';

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
    MatProgressSpinnerModule,
    LoadingSpinner,
  ],
  templateUrl: './inventory-movement-dialog.html',
  styleUrl: './inventory-movement-dialog.scss',
})
export class InventoryMovementDialog implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly inventoryService = inject(InventoryService);
  private readonly productsService = inject(ProductsService);
  private readonly toastr = inject(ToastrService);
  private readonly dialogRef = inject(MatDialogRef<InventoryMovementDialog, boolean>);
  readonly data = inject<InventoryMovementDialogData>(MAT_DIALOG_DATA);

  readonly products = signal<ProductDto[]>([]);
  readonly productsLoading = signal(true);
  readonly submitting = signal(false);

  readonly typeOptions = TRANSACTION_TYPE_FORM_OPTIONS;
  readonly transactionType = TransactionType;

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

  ngOnInit(): void {
    this.loadProducts();

    this.form.controls.productId.valueChanges.subscribe(() =>
      this.form.controls.quantity.updateValueAndValidity(),
    );
    this.form.controls.transactionType.valueChanges.subscribe(() =>
      this.form.controls.quantity.updateValueAndValidity(),
    );
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
      .pipe(finalize(() => this.submitting.set(false)))
      .subscribe({
        next: (result) => {
          const label = getTransactionTypeLabel(transactionType);
          this.toastr.success(
            `${label}: ${result.quantity} units for ${result.productName}`,
            'Inventory updated',
          );
          this.dialogRef.close(true);
        },
        error: (err) => {
          const message = err?.error?.message ?? 'Failed to record inventory movement.';
          this.toastr.error(message);
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

  private loadProducts(): void {
    this.productsLoading.set(true);
    this.productsService
      .getProducts({ pageNumber: 1, pageSize: 100 })
      .pipe(finalize(() => this.productsLoading.set(false)))
      .subscribe({
        next: (response) => {
          this.products.set(response.items);
          if (this.data.productId && response.items.some((p) => p.id === this.data.productId)) {
            this.form.patchValue({ productId: this.data.productId });
          } else if (!this.data.productId && response.items.length > 0) {
            this.form.patchValue({ productId: response.items[0].id });
          }
        },
        error: () => {
          this.toastr.error('Failed to load products for selection.');
        },
      });
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
