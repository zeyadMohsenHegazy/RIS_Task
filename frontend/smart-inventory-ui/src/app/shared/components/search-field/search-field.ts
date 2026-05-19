import { Component, input } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-search-field',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
  ],
  template: `
    <mat-form-field appearance="outline" class="search-field">
      <mat-label>{{ label() }}</mat-label>
      <input
        matInput
        [formControl]="control()"
        [placeholder]="placeholder()"
        [attr.aria-label]="label()"
      />
      <mat-icon matPrefix>search</mat-icon>
      @if (control().value) {
        <button
          mat-icon-button
          matSuffix
          type="button"
          aria-label="Clear search"
          (click)="clear()"
        >
          <mat-icon>close</mat-icon>
        </button>
      }
    </mat-form-field>
  `,
  styles: `
    .search-field {
      flex: 1;
      min-width: 220px;
      max-width: 400px;
    }
  `,
})
export class SearchField {
  readonly label = input('Search');
  readonly placeholder = input('');
  readonly control = input.required<FormControl<string>>();

  clear(): void {
    this.control().setValue('');
  }
}
