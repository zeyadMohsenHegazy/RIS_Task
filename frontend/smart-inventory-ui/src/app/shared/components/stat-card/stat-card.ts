import { Component, input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { SkeletonBlock } from '../skeleton/skeleton-block';

export type StatCardAccent = 'primary' | 'accent' | 'warn' | 'neutral';

@Component({
  selector: 'app-stat-card',
  imports: [MatCardModule, MatIconModule, SkeletonBlock],
  templateUrl: './stat-card.html',
  styleUrl: './stat-card.scss',
})
export class StatCard {
  readonly label = input.required<string>();
  readonly value = input<number | string | null>(null);
  readonly icon = input.required<string>();
  readonly accent = input<StatCardAccent>('primary');
  readonly loading = input(false);
}
