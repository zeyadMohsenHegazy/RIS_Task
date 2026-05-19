import { Component, input } from '@angular/core';

export type SkeletonVariant = 'text' | 'rect' | 'circle';

@Component({
  selector: 'app-skeleton-block',
  template: `<div
    class="skeleton-block"
    [class]="'skeleton-block--' + variant()"
    [style.width]="width()"
    [style.height]="height()"
    aria-hidden="true"
  ></div>`,
  styles: `
    :host {
      display: block;
    }

    .skeleton-block {
      background: linear-gradient(
        90deg,
        color-mix(in srgb, currentColor 6%, transparent) 0%,
        color-mix(in srgb, currentColor 12%, transparent) 50%,
        color-mix(in srgb, currentColor 6%, transparent) 100%
      );
      background-size: 200% 100%;
      animation: skeleton-shimmer 1.4s ease-in-out infinite;
      border-radius: 6px;
    }

    .skeleton-block--text {
      border-radius: 4px;
      height: 0.875rem;
    }

    .skeleton-block--circle {
      border-radius: 50%;
      aspect-ratio: 1;
    }

    @keyframes skeleton-shimmer {
      0% {
        background-position: 200% 0;
      }
      100% {
        background-position: -200% 0;
      }
    }
  `,
})
export class SkeletonBlock {
  readonly variant = input<SkeletonVariant>('text');
  readonly width = input<string>('100%');
  readonly height = input<string | undefined>(undefined);
}
