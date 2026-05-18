import { Component, inject, output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { AuthService } from '../../features/auth/auth.service';
import { environment } from '../../../environments/environment';
import { MAIN_NAV_ITEMS } from '../navigation.config';

@Component({
  selector: 'app-sidebar',
  imports: [
    RouterLink,
    RouterLinkActive,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatDividerModule,
  ],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class Sidebar {
  readonly auth = inject(AuthService);
  readonly appName = environment.appName;
  readonly navItems = MAIN_NAV_ITEMS;

  readonly navigate = output<void>();

  onNavClick(): void {
    this.navigate.emit();
  }

  logout(): void {
    this.auth.logout();
  }

  userInitials(email: string): string {
    const part = email.split('@')[0] ?? 'U';
    return part.slice(0, 2).toUpperCase();
  }
}
