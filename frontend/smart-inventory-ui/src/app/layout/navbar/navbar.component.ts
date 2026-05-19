import { Component, computed, inject, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink } from '@angular/router';
import { BRAND } from '../../core/brand.config';
import { AuthService } from '../../features/auth/auth.service';
import { ThemeService } from '../../theme/theme.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, MatToolbarModule, MatButtonModule, MatIconModule, MatMenuModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
})
export class NavbarComponent {
  readonly auth = inject(AuthService);
  readonly theme = inject(ThemeService);
  readonly brand = BRAND;

  readonly pageTitle = input('Home');
  readonly showMenuButton = input(false);

  readonly menuToggle = output<void>();

  readonly displayRole = computed(() => {
    const roles = this.auth.roles();
    if (roles.includes('Admin')) {
      return 'Admin';
    }
    if (roles.includes('Employee')) {
      return 'Employee';
    }
    return roles.join(', ') || 'User';
  });

  onMenuClick(): void {
    this.menuToggle.emit();
  }

  logout(): void {
    this.auth.logout();
  }
}

/** @deprecated Use NavbarComponent — was TopNavbar */
export const TopNavbar = NavbarComponent;
