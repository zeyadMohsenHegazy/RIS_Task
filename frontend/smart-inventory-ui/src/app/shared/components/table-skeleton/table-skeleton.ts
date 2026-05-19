import { Component, computed, input } from '@angular/core';
import { SkeletonBlock } from '../skeleton/skeleton-block';

@Component({
  selector: 'app-table-skeleton',
  imports: [SkeletonBlock],
  templateUrl: './table-skeleton.html',
  styleUrl: './table-skeleton.scss',
  host: {
    '[style.--skeleton-cols]': 'columns()',
  },
})
export class TableSkeleton {
  readonly columns = input(5);
  readonly rows = input(6);
  readonly showHeader = input(true);

  readonly columnIndices = computed(() =>
    Array.from({ length: this.columns() }, (_, index) => index),
  );

  readonly rowIndices = computed(() =>
    Array.from({ length: this.rows() }, (_, index) => index),
  );
}
