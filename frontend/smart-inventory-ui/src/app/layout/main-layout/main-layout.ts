import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component, computed, inject, viewChild } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { filter, map } from 'rxjs';
import { Sidebar } from '../sidebar/sidebar';
import { TopNavbar } from '../top-navbar/top-navbar';
import { MAIN_NAV_ITEMS } from '../navigation.config';

@Component({
  selector: 'app-main-layout',
  imports: [RouterOutlet, MatSidenavModule, Sidebar, TopNavbar],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss',
})
export class MainLayout {
  private readonly breakpoint = inject(BreakpointObserver);
  private readonly router = inject(Router);

  private readonly drawer = viewChild<MatSidenav>('drawer');

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
    { initialValue: this.resolvePageTitle() },
  );

  toggleSidenav(): void {
    this.drawer()?.toggle();
  }

  closeSidenavOnMobile(): void {
    if (this.isHandset()) {
      this.drawer()?.close();
    }
  }

  private resolvePageTitle(): string {
    let route = this.router.routerState.root;
    while (route.firstChild) {
      route = route.firstChild;
    }

    const dataTitle = route.snapshot.data['title'];
    if (typeof dataTitle === 'string') {
      return dataTitle;
    }

    const url = this.router.url.split('?')[0];
    const match = MAIN_NAV_ITEMS.find((item) => url.startsWith(item.route));
    return match?.label ?? 'Smart Inventory';
  }
}
