import { inject, Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { InventoryMovementDialogData } from '../../models/inventory.model';
import { InventoryMovementDialog } from './inventory-movement-dialog/inventory-movement-dialog';

@Injectable({ providedIn: 'root' })
export class InventoryMovementDialogService {
  private readonly dialog = inject(MatDialog);

  open(data: InventoryMovementDialogData = {}): Observable<boolean> {
    return this.dialog
      .open(InventoryMovementDialog, {
        width: '520px',
        maxWidth: '95vw',
        data,
        autoFocus: 'dialog',
        restoreFocus: true,
      })
      .afterClosed()
      .pipe(map((result) => result === true));
  }
}
