import { Component, inject, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatToolbarModule } from '@angular/material/toolbar';
import { AuthService } from '../../features/auth/auth.service';
import { ThemeService } from '../../theme/theme.service';

@Component({
  selector: 'app-top-navbar',
  imports: [MatToolbarModule, MatButtonModule, MatIconModule, MatMenuModule],
  templateUrl: './top-navbar.html',
  styleUrl: './top-navbar.scss',
})
export class TopNavbar {
  readonly auth = inject(AuthService);
  readonly theme = inject(ThemeService);

  readonly pageTitle = input('Dashboard');
  readonly showMenuButton = input(false);

  readonly menuToggle = output<void>();

  onMenuClick(): void {
    this.menuToggle.emit();
  }

  logout(): void {
    this.auth.logout();
  }
}
