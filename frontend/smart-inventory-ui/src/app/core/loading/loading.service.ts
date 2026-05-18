import { computed, Injectable, signal } from '@angular/core';

const SHOW_DELAY_MS = 280;

@Injectable({ providedIn: 'root' })
export class LoadingService {
  private readonly pendingCount = signal(0);
  private readonly debouncedVisible = signal(false);
  private readonly message = signal<string | undefined>(undefined);

  private showTimer: ReturnType<typeof setTimeout> | null = null;

  /** True while at least one tracked HTTP request is in flight (immediate). */
  readonly isActive = computed(() => this.pendingCount() > 0);

  /** True when the global overlay should be visible (debounced). */
  readonly visible = this.debouncedVisible.asReadonly();

  readonly globalMessage = this.message.asReadonly();

  /** Manual global loader (non-HTTP flows). */
  show(message?: string): void {
    this.message.set(message);
    this.increment();
  }

  hide(): void {
    this.decrement();
  }

  trackStart(message?: string): void {
    if (message) {
      this.message.set(message);
    }
    this.increment();
  }

  trackEnd(): void {
    this.decrement();
  }

  private increment(): void {
    const next = this.pendingCount() + 1;
    this.pendingCount.set(next);

    if (next === 1) {
      this.showTimer = setTimeout(() => {
        if (this.pendingCount() > 0) {
          this.debouncedVisible.set(true);
        }
      }, SHOW_DELAY_MS);
    }
  }

  private decrement(): void {
    const next = Math.max(0, this.pendingCount() - 1);
    this.pendingCount.set(next);

    if (next === 0) {
      if (this.showTimer) {
        clearTimeout(this.showTimer);
        this.showTimer = null;
      }
      this.debouncedVisible.set(false);
      this.message.set(undefined);
    }
  }
}
