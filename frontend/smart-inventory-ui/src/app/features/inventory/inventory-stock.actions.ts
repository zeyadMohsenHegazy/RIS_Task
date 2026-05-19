import { DestroyRef, inject, Injectable } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TransactionType } from '../../models/inventory.model';
import { ProductDto } from '../../models/product.model';
import { InventoryMovementDialogService } from './inventory-movement-dialog.service';

@Injectable({ providedIn: 'root' })
export class InventoryStockActions {
  private readonly dialog = inject(InventoryMovementDialogService);

  open(
    type: TransactionType,
    destroyRef: DestroyRef,
    options?: { product?: ProductDto; onSuccess?: () => void },
  ): void {
    this.dialog
      .open({
        transactionType: type,
        productId: options?.product?.id,
      })
      .pipe(takeUntilDestroyed(destroyRef))
      .subscribe((success) => {
        if (success) {
          options?.onSuccess?.();
        }
      });
  }
}
