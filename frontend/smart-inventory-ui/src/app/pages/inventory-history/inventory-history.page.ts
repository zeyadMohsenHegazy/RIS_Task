import { Component, computed, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { PageEvent } from '@angular/material/paginator';
import { AuthService } from '../../features/auth/auth.service';
import { InventoryHistoryStore } from '../../features/inventory/inventory-history.store';
import { InventoryStockActions } from '../../features/inventory/inventory-stock.actions';
import {
  getTransactionTypeLabel,
  TRANSACTION_TYPE_FILTER_OPTIONS,
  TransactionType,
} from '../../models/inventory.model';
import {
  bindDebouncedSearch,
  ErrorState,
  formatDateTime,
  PageHeader,
  PaginatedTableShell,
  PAGE_SIZE_OPTIONS,
  SearchField,
  StockActionButtons,
} from '../../shared';

@Component({
  selector: 'app-inventory-history-page',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatTableModule,
    MatFormFieldModule,
    MatSelectModule,
    MatIconModule,
    PageHeader,
    SearchField,
    StockActionButtons,
    PaginatedTableShell,
    ErrorState,
  ],
  templateUrl: './inventory-history.page.html',
  styleUrl: './inventory-history.page.scss',
})
export class InventoryHistoryPage implements OnInit {
  private readonly historyStore = inject(InventoryHistoryStore);
  private readonly stockActions = inject(InventoryStockActions);
  private readonly auth = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);

  readonly isAdmin = this.auth.isAdmin;
  readonly searchControl = new FormControl('', { nonNullable: true });
  readonly typeFilterControl = new FormControl<TransactionType | null>(null);

  readonly transactions = this.historyStore.transactions;
  readonly totalCount = this.historyStore.totalCount;
  readonly loading = this.historyStore.loading;
  readonly error = this.historyStore.error;

  readonly pageIndex = computed(() => this.historyStore.historyQuery().pageNumber - 1);
  readonly pageSize = computed(() => this.historyStore.historyQuery().pageSize);

  readonly typeFilterOptions = TRANSACTION_TYPE_FILTER_OPTIONS;
  readonly displayedColumns = [
    'productName',
    'transactionType',
    'quantity',
    'createdByUsername',
    'transactionDate',
  ] as const;
  readonly pageSizeOptions = PAGE_SIZE_OPTIONS;
  readonly transactionType = TransactionType;
  readonly getTypeLabel = getTransactionTypeLabel;
  readonly formatDate = formatDateTime;

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
    this.stockActions.open(TransactionType.In, this.destroyRef, {
      onSuccess: () => this.historyStore.invalidate(),
    });
  }

  onStockOut(): void {
    this.stockActions.open(TransactionType.Out, this.destroyRef, {
      onSuccess: () => this.historyStore.invalidate(),
    });
  }

  private setupFilters(): void {
    bindDebouncedSearch(this.searchControl, this.destroyRef, (search) => {
      this.historyStore.setQuery({ pageNumber: 1, search });
    });

    this.typeFilterControl.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((transactionType) => {
        this.historyStore.setQuery({ pageNumber: 1, transactionType });
      });
  }
}
