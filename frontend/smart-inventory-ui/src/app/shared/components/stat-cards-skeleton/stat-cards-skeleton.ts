import { Component, input } from '@angular/core';
import { SkeletonBlock } from '../skeleton/skeleton-block';

@Component({
  selector: 'app-stat-cards-skeleton',
  imports: [SkeletonBlock],
  template: `
    <div class="stat-cards-skeleton" role="status" aria-label="Loading statistics">
      @for (card of cards; track $index) {
        <div class="stat-cards-skeleton__card">
          <app-skeleton-block variant="text" width="55%" height="12px" />
          <app-skeleton-block variant="text" width="40%" height="28px" />
          <app-skeleton-block variant="text" width="70%" height="10px" />
        </div>
      }
    </div>
  `,
  styles: `
    .stat-cards-skeleton {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
      gap: 1rem;
    }

    .stat-cards-skeleton__card {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
      padding: 1.25rem;
      border-radius: 12px;
      border: 1px solid var(--sim-border);
      background: var(--sim-surface);
    }
  `,
})
export class StatCardsSkeleton {
  readonly count = input(4);

  get cards(): number[] {
    return Array.from({ length: this.count() }, (_, i) => i);
  }
}
