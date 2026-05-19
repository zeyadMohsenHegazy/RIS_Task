import { inject, Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { ConfirmDialog } from './confirm-dialog';
import { ConfirmDialogData } from './confirm-dialog.model';

@Injectable({ providedIn: 'root' })
export class ConfirmDialogService {
  private readonly dialog = inject(MatDialog);

  confirm(data: ConfirmDialogData): Observable<boolean> {
    return this.dialog
      .open(ConfirmDialog, {
        width: '400px',
        maxWidth: '95vw',
        data,
        autoFocus: 'dialog',
        restoreFocus: true,
      })
      .afterClosed();
  }
}
