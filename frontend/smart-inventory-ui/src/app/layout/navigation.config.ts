export interface NavItem {
  label: string;
  icon: string;
  route: string;
}

export const MAIN_NAV_ITEMS: NavItem[] = [
  { label: 'Dashboard', icon: 'dashboard', route: '/dashboard' },
  { label: 'Products', icon: 'inventory_2', route: '/products' },
  { label: 'Inventory History', icon: 'history', route: '/inventory/history' },
];
