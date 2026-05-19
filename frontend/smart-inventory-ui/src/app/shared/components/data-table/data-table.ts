import { Component, computed, input } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { TableColumn } from '../../models/table-column.model';
import { TableSkeleton } from '../table-skeleton/table-skeleton';

@Component({
  selector: 'app-data-table',
  imports: [MatTableModule, MatSortModule, TableSkeleton],
  templateUrl: './data-table.html',
  styleUrl: './data-table.scss',
})
export class DataTable<T extends object> {
  readonly columns = input.required<TableColumn<T>[]>();
  readonly data = input<T[]>([]);
  readonly loading = input(false);
  readonly emptyMessage = input('No data available');

  readonly displayedColumns = computed(() => this.columns().map((c) => c.key));

  getCellValue(row: T, column: TableColumn<T>): string {
    if (column.format) {
      return column.format(row);
    }
    const value = (row as Record<string, unknown>)[column.key];
    return value == null ? '' : String(value);
  }
}
