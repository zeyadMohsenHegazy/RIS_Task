import { Component, computed, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { AuthService } from '../../features/auth/auth.service';
import { InventoryHistoryStore } from '../../features/inventory/inventory-history.store';
import { InventoryMovementDialogService } from '../../features/inventory/inventory-movement-dialog.service';
import {
  getTransactionTypeLabel,
  TRANSACTION_TYPE_FILTER_OPTIONS,
  TransactionType,
} from '../../models/inventory.model';
import { ErrorState, TableSkeleton } from '../../shared';

@Component({
  selector: 'app-inventory-history-page',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    ErrorState,
    TableSkeleton,
  ],
  templateUrl: './inventory-history.page.html',
  styleUrl: './inventory-history.page.scss',
})
export class InventoryHistoryPage implements OnInit {
  private readonly historyStore = inject(InventoryHistoryStore);
  private readonly inventoryDialog = inject(InventoryMovementDialogService);
  private readonly auth = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);

  readonly isAdmin = this.auth.isAdmin;

  readonly searchControl = new FormControl('', { nonNullable: true });
  readonly typeFilterControl = new FormControl<TransactionType | null>(null);

  readonly transactions = this.historyStore.transactions;
  readonly totalCount = this.historyStore.totalCount;
  readonly loading = this.historyStore.loading;
  readonly error = this.historyStore.error;

  readonly pageIndex = computed(
    () => this.historyStore.historyQuery().pageNumber - 1,
  );
  readonly pageSize = computed(() => this.historyStore.historyQuery().pageSize);

  readonly typeFilterOptions = TRANSACTION_TYPE_FILTER_OPTIONS;
  readonly displayedColumns = [
    'productName',
    'transactionType',
    'quantity',
    'createdByUsername',
    'transactionDate',
  ];
  readonly pageSizeOptions = [5, 10, 25, 50];
  readonly transactionType = TransactionType;

  ngOnInit(): void {
    const query = this.historyStore.historyQuery();
    this.searchControl.setValue(query.search, { emitEvent: false });
    this.typeFilterControl.setValue(query.transactionType, { emitEvent: false });
    this.setupFilters();
    this.historyStore.load();
  }

  onPageChange(event: PageEvent): void {
    this.historyStore.setQuery({
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize,
    });
  }

  loadHistory(): void {
    this.historyStore.load();
  }

  onStockIn(): void {
    this.openMovementDialog(TransactionType.In);
  }

  onStockOut(): void {
    this.openMovementDialog(TransactionType.Out);
  }

  private openMovementDialog(type: TransactionType): void {
    this.inventoryDialog
      .open({ transactionType: type })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((success) => {
        if (success) {
          this.historyStore.invalidate();
        }
      });
  }

  getTypeLabel(type: TransactionType): string {
    return getTransactionTypeLabel(type);
  }

  formatDate(isoDate: string): string {
    return new Date(isoDate).toLocaleString();
  }

  private setupFilters(): void {
    this.searchControl.valueChanges
      .pipe(debounceTime(350), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe((search) => {
        this.historyStore.setQuery({ pageNumber: 1, search });
      });

    this.typeFilterControl.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((transactionType) => {
        this.historyStore.setQuery({ pageNumber: 1, transactionType });
      });
  }
}
