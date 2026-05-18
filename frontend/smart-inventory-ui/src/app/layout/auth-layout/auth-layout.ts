import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-auth-layout',
  imports: [RouterOutlet],
  template: '<router-outlet />',
  styles: `
    :host {
      display: block;
      min-height: 100vh;
      background: var(--sim-page-bg, #f5f7fa);
      color: inherit;
    }
  `,
})
export class AuthLayout {}
