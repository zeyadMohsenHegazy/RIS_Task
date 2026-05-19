import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component, computed, inject, signal, viewChild } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { filter, map } from 'rxjs';
import { AuthService } from '../../features/auth/auth.service';
import { getNavItemsForRoles } from '../navigation.config';
import { NavbarComponent } from '../navbar/navbar.component';
import { SidebarComponent } from '../sidebar/sidebar.component';

@Component({
  selector: 'app-main-layout',
  imports: [RouterOutlet, MatSidenavModule, SidebarComponent, NavbarComponent],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss',
})
export class MainLayoutComponent {
  private readonly breakpoint = inject(BreakpointObserver);
  private readonly router = inject(Router);
  private readonly auth = inject(AuthService);

  private readonly drawer = viewChild<MatSidenav>('drawer');

  protected readonly sidebarCollapsed = signal(false);

  protected readonly isHandset = toSignal(
    this.breakpoint.observe(Breakpoints.Handset).pipe(map((state) => state.matches)),
    { initialValue: false },
  );

  readonly sidenavMode = computed<'over' | 'side'>(() =>
    this.isHandset() ? 'over' : 'side',
  );

  readonly showMenuButton = computed(() => this.isHandset());

  readonly pageTitle = toSignal(
    this.router.events.pipe(
      filter((event) => event instanceof NavigationEnd),
      map(() => this.resolvePageTitle()),
    ),
    { initialValue: 'Smart Inventory' },
  );

  toggleSidenav(): void {
    if (this.isHandset()) {
      this.drawer()?.toggle();
    } else {
      this.sidebarCollapsed.update((v) => !v);
    }
  }

  closeSidenavOnMobile(): void {
    if (this.isHandset()) {
      this.drawer()?.close();
    }
  }

  toggleSidebarCollapse(): void {
    this.sidebarCollapsed.update((collapsed) => !collapsed);
  }

  private resolvePageTitle(): string {
    let snapshot = this.router.routerState.snapshot.root;

    while (snapshot.firstChild) {
      snapshot = snapshot.firstChild;
    }

    const dataTitle = snapshot.data['title'];
    if (typeof dataTitle === 'string') {
      return dataTitle;
    }

    const url = this.router.url.split('?')[0];
    const items = getNavItemsForRoles(this.auth.roles());
    const match = items.find((item) => url === item.route || url.startsWith(item.route + '/'));
    return match?.label ?? 'Smart Inventory';
  }
}

/** @deprecated Use MainLayoutComponent */
export const MainLayout = MainLayoutComponent;
