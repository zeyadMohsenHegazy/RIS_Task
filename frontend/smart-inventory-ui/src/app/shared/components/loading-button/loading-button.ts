import { Component, computed, input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

export type LoadingButtonVariant = 'flat' | 'stroked' | 'raised' | 'basic';

@Component({
  selector: 'app-loading-button',
  imports: [MatButtonModule, MatProgressSpinnerModule],
  template: `
    @switch (variant()) {
      @case ('stroked') {
        <button
          mat-stroked-button
          [type]="type()"
          [color]="color()"
          [disabled]="isDisabled()"
          [attr.aria-busy]="loading()"
        >
          @if (loading()) {
            <span class="loading-button__inner">
              <mat-spinner [diameter]="spinnerDiameter()" />
              @if (loadingLabel()) {
                <span>{{ loadingLabel() }}</span>
              }
            </span>
          } @else {
            <ng-content />
          }
        </button>
      }
      @case ('raised') {
        <button
          mat-raised-button
          [type]="type()"
          [color]="color()"
          [disabled]="isDisabled()"
          [attr.aria-busy]="loading()"
        >
          @if (loading()) {
            <span class="loading-button__inner">
              <mat-spinner [diameter]="spinnerDiameter()" />
              @if (loadingLabel()) {
                <span>{{ loadingLabel() }}</span>
              }
            </span>
          } @else {
            <ng-content />
          }
        </button>
      }
      @case ('basic') {
        <button
          mat-button
          [type]="type()"
          [color]="color()"
          [disabled]="isDisabled()"
          [attr.aria-busy]="loading()"
        >
          @if (loading()) {
            <span class="loading-button__inner">
              <mat-spinner [diameter]="spinnerDiameter()" />
              @if (loadingLabel()) {
                <span>{{ loadingLabel() }}</span>
              }
            </span>
          } @else {
            <ng-content />
          }
        </button>
      }
      @default {
        <button
          mat-flat-button
          [type]="type()"
          [color]="color()"
          [disabled]="isDisabled()"
          [attr.aria-busy]="loading()"
        >
          @if (loading()) {
            <span class="loading-button__inner">
              <mat-spinner [diameter]="spinnerDiameter()" />
              @if (loadingLabel()) {
                <span>{{ loadingLabel() }}</span>
              }
            </span>
          } @else {
            <ng-content />
          }
        </button>
      }
    }
  `,
  styles: `
    .loading-button__inner {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
    }

    mat-spinner {
      display: inline-block;
    }
  `,
})
export class LoadingButton {
  readonly loading = input(false);
  readonly disabled = input(false);
  readonly variant = input<LoadingButtonVariant>('flat');
  readonly color = input<'primary' | 'accent' | 'warn' | undefined>('primary');
  readonly type = input<'button' | 'submit' | 'reset'>('button');
  readonly loadingLabel = input<string | undefined>(undefined);
  readonly spinnerDiameter = input(20);

  readonly isDisabled = computed(() => this.disabled() || this.loading());
}
