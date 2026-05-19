import { Component, inject } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { LoadingService } from '../../../core/loading/loading.service';

@Component({
  selector: 'app-global-loader',
  imports: [MatProgressSpinnerModule],
  template: `
    @if (loading.visible()) {
      <div class="global-loader" role="status" aria-live="polite" aria-busy="true">
        <div class="global-loader__panel">
          <mat-spinner diameter="48" />
          @if (loading.globalMessage(); as msg) {
            <p class="global-loader__message">{{ msg }}</p>
          }
        </div>
      </div>
    }
  `,
  styles: `
    .global-loader {
      position: fixed;
      inset: 0;
      z-index: 10000;
      display: flex;
      align-items: center;
      justify-content: center;
      background: color-mix(in srgb, var(--sim-bg, #0f1419) 35%, transparent);
      backdrop-filter: blur(2px);
      animation: global-loader-fade-in 0.2s ease-out;
      pointer-events: all;
    }

    .global-loader__panel {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 1rem;
      padding: 1.75rem 2rem;
      border-radius: 12px;
      background: var(--sim-surface, #1a2332);
      border: 1px solid var(--sim-border, rgba(255, 255, 255, 0.08));
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.24);
    }

    .global-loader__message {
      margin: 0;
      font-size: 0.875rem;
      color: var(--sim-text-muted, rgba(255, 255, 255, 0.7));
    }

    @keyframes global-loader-fade-in {
      from {
        opacity: 0;
      }
      to {
        opacity: 1;
      }
    }
  `,
})
export class GlobalLoader {
  protected readonly loading = inject(LoadingService);
}
