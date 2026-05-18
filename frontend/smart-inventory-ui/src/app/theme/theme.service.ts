import { Injectable, signal } from '@angular/core';

const THEME_STORAGE_KEY = 'sim-theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  readonly isDark = signal(true);

  constructor() {
    const saved = localStorage.getItem(THEME_STORAGE_KEY);
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

  private applyTheme(isDark: boolean): void {
    this.isDark.set(isDark);
    const root = document.documentElement;
    root.classList.toggle('dark-theme', isDark);
    root.classList.toggle('light-theme', !isDark);
    localStorage.setItem(THEME_STORAGE_KEY, isDark ? 'dark' : 'light');
  }
}
