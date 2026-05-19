import { Component, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-error-state',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './error-state.html',
  styleUrl: './error-state.scss',
})
export class ErrorState {
  readonly title = input('Something went wrong');
  readonly message = input('An unexpected error occurred. Please try again.');
  readonly icon = input('error_outline');
  readonly showRetry = input(true);
  readonly retryLabel = input('Try again');

  readonly retry = output<void>();
}
