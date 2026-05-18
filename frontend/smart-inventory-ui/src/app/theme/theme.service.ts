import { computed, Injectable, signal } from '@angular/core';

const THEME_STORAGE_KEY = 'sim-theme';

export type ThemeMode = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  readonly isDark = signal(true);
  readonly mode = computed<ThemeMode>(() => (this.isDark() ? 'dark' : 'light'));

  constructor() {
    const saved = localStorage.getItem(THEME_STORAGE_KEY) as ThemeMode | null;
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    const isDark =
      saved === 'dark' ? true : saved === 'light' ? false : prefersDark;
    this.applyTheme(isDark);
  }

  toggle(): void {
    this.applyTheme(!this.isDark());
  }

  setDarkMode(isDark: boolean): void {
    this.applyTheme(isDark);
  }

  setMode(mode: ThemeMode): void {
    this.applyTheme(mode === 'dark');
  }

  private applyTheme(isDark: boolean): void {
    this.isDark.set(isDark);
    const root = document.documentElement;
    root.classList.toggle('dark-theme', isDark);
    root.classList.toggle('light-theme', !isDark);
    localStorage.setItem(THEME_STORAGE_KEY, isDark ? 'dark' : 'light');
  }
}
