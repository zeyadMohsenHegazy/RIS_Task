import { Component, computed, inject, input, output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatTooltipModule } from '@angular/material/tooltip';
import { BRAND } from '../../core/brand.config';
import { AuthService } from '../../features/auth/auth.service';
import { getNavItemsForRoles } from '../navigation.config';

@Component({
  selector: 'app-sidebar',
  imports: [
    RouterLink,
    RouterLinkActive,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatDividerModule,
    MatTooltipModule,
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {
  readonly auth = inject(AuthService);
  readonly brand = BRAND;

  readonly collapsed = input(false);

  readonly navigate = output<void>();
  readonly toggleCollapse = output<void>();

  readonly navItems = computed(() =>
    getNavItemsForRoles(this.auth.roles()),
  );

  readonly displayRole = computed(() => {
    const roles = this.auth.roles();
    if (roles.includes('Admin')) {
      return 'Admin';
    }
    if (roles.includes('Employee')) {
      return 'Employee';
    }
    return roles[0] ?? 'User';
  });

  onNavClick(): void {
    this.navigate.emit();
  }

  onToggleCollapse(): void {
    this.toggleCollapse.emit();
  }

  logout(): void {
    this.auth.logout();
  }

  userInitials(name: string): string {
    const part = name.split(/[@\s]/)[0] ?? 'U';
    return part.slice(0, 2).toUpperCase();
  }
}

/** @deprecated Use SidebarComponent */
export const Sidebar = SidebarComponent;
