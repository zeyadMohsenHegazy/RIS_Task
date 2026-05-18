import {
  Component,
  DestroyRef,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { debounceTime, distinctUntilChanged, finalize, Subject, switchMap, tap } from 'rxjs';
import { AuthService } from '../../features/auth/auth.service';
import { InventoryMovementDialogService } from '../../features/inventory/inventory-movement-dialog.service';
import { InventoryService } from '../../features/inventory/inventory.service';
import {
  getTransactionTypeLabel,
  InventoryTransactionDto,
  TRANSACTION_TYPE_FILTER_OPTIONS,
  TransactionType,
} from '../../models/inventory.model';
import { ErrorState, LoadingSpinner } from '../../shared';

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
    LoadingSpinner,
  ],
  templateUrl: './inventory-history.page.html',
  styleUrl: './inventory-history.page.scss',
})
export class InventoryHistoryPage implements OnInit {
  private readonly inventoryService = inject(InventoryService);
  private readonly inventoryDialog = inject(InventoryMovementDialogService);
  private readonly auth = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);

  readonly isAdmin = this.auth.isAdmin;

  private readonly load$ = new Subject<void>();

  readonly searchControl = new FormControl('', { nonNullable: true });
  readonly typeFilterControl = new FormControl<TransactionType | null>(null);

  readonly transactions = signal<InventoryTransactionDto[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly pageIndex = signal(0);
  readonly pageSize = signal(10);

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
    this.setupHistoryLoader();
    this.setupFilters();
    this.loadHistory();
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadHistory();
  }

  loadHistory(): void {
    this.load$.next();
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
          this.loadHistory();
        }
      });
  }

  getTypeLabel(type: TransactionType): string {
    return getTransactionTypeLabel(type);
  }

  formatDate(isoDate: string): string {
    return new Date(isoDate).toLocaleString();
  }

  private setupHistoryLoader(): void {
    this.load$
      .pipe(
        tap(() => {
          this.loading.set(true);
          this.error.set(null);
        }),
        switchMap(() =>
          this.inventoryService
            .getHistory({
              pageNumber: this.pageIndex() + 1,
              pageSize: this.pageSize(),
              search: this.searchControl.value,
              transactionType: this.typeFilterControl.value,
            })
            .pipe(finalize(() => this.loading.set(false))),
        ),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (response) => {
          this.transactions.set(response.items);
          this.totalCount.set(response.totalCount);
        },
        error: () => {
          this.error.set('Unable to load inventory history. Please try again.');
          this.transactions.set([]);
          this.totalCount.set(0);
        },
      });
  }

  private setupFilters(): void {
    this.searchControl.valueChanges
      .pipe(debounceTime(350), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.pageIndex.set(0);
        this.loadHistory();
      });

    this.typeFilterControl.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.pageIndex.set(0);
        this.loadHistory();
      });
  }
}
