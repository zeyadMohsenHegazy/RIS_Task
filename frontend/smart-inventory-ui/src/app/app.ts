import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './features/auth/auth.service';
import { GlobalLoader } from './shared/components/global-loader/global-loader';
import { ThemeService } from './theme/theme.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, GlobalLoader],
  template: `
    <app-global-loader />
    <router-outlet />
  `,
  styleUrl: './app.scss',
})
export class App {
  constructor() {
    inject(ThemeService);
    inject(AuthService);
  }
}
