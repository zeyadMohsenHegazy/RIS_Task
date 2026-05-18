import { Component, input, output } from '@angular/core';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { PAGE_SIZE_OPTIONS } from '../../constants/pagination.constants';
import { TableSkeleton } from '../table-skeleton/table-skeleton';

@Component({
  selector: 'app-paginated-table-shell',
  imports: [TableSkeleton, MatPaginatorModule],
  templateUrl: './paginated-table-shell.html',
  styleUrl: './paginated-table-shell.scss',
})
export class PaginatedTableShell {
  readonly loading = input(false);
  readonly totalCount = input(0);
  readonly pageIndex = input(0);
  readonly pageSize = input(10);
  readonly columnCount = input(5);
  readonly skeletonRows = input(8);
  readonly paginatorLabel = input('Table pagination');
  readonly pageSizeOptions = input<readonly number[]>(PAGE_SIZE_OPTIONS);

  readonly pageChange = output<PageEvent>();

  onPageChange(event: PageEvent): void {
    this.pageChange.emit(event);
  }
}
