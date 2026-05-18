import { Component, input } from '@angular/core';
import { SkeletonBlock } from '../skeleton/skeleton-block';

@Component({
  selector: 'app-form-skeleton',
  imports: [SkeletonBlock],
  template: `
    <div class="form-skeleton" role="status" aria-label="Loading form">
      @for (field of fields; track $index) {
        <div class="form-skeleton__field">
          <app-skeleton-block width="30%" height="12px" />
          <app-skeleton-block height="42px" />
        </div>
      }
      <div class="form-skeleton__actions">
        <app-skeleton-block width="88px" height="36px" />
        <app-skeleton-block width="72px" height="36px" />
      </div>
    </div>
  `,
  styles: `
    .form-skeleton {
      display: flex;
      flex-direction: column;
      gap: 1.25rem;
      max-width: 480px;
    }

    .form-skeleton__field {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .form-skeleton__actions {
      display: flex;
      gap: 0.75rem;
      margin-top: 0.5rem;
    }
  `,
})
export class FormSkeleton {
  readonly fieldCount = input(5);

  get fields(): number[] {
    return Array.from({ length: this.fieldCount() }, (_, i) => i);
  }
}
