import { Component, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-stock-action-buttons',
  imports: [MatButtonModule, MatIconModule],
  template: `
    <button mat-stroked-button type="button" (click)="stockIn.emit()">
      <mat-icon>add_circle</mat-icon>
      Stock In
    </button>
    <button mat-stroked-button color="warn" type="button" (click)="stockOut.emit()">
      <mat-icon>remove_circle</mat-icon>
      Stock Out
    </button>
  `,
  styles: `
    :host {
      display: contents;
    }
  `,
})
export class StockActionButtons {
  readonly stockIn = output<void>();
  readonly stockOut = output<void>();
}
