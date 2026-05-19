export interface NavItem {
  label: string;
  icon: string;
  route: string;
  /** When set, only users with at least one of these roles see the item. */
  roles?: string[];
}

export const ADMIN_NAV_ITEMS: NavItem[] = [
  { label: 'Home', icon: 'home', route: '/home' },
  { label: 'Dashboard', icon: 'dashboard', route: '/dashboard' },
  { label: 'Products', icon: 'inventory_2', route: '/products' },
  { label: 'Add Product', icon: 'add_box', route: '/products/new', roles: ['Admin'] },
  { label: 'Inventory History', icon: 'history', route: '/inventory/history' },
  { label: 'Warehouses', icon: 'warehouse', route: '/warehouses', roles: ['Admin'] },
  { label: 'Users', icon: 'group', route: '/users', roles: ['Admin'] },
  { label: 'Add User', icon: 'person_add', route: '/users/new', roles: ['Admin'] },
];

export const EMPLOYEE_NAV_ITEMS: NavItem[] = [
  { label: 'Home', icon: 'home', route: '/home' },
  { label: 'Dashboard', icon: 'dashboard', route: '/dashboard' },
  { label: 'Products', icon: 'inventory_2', route: '/products' },
  { label: 'Inventory History', icon: 'history', route: '/inventory/history' },
];

/** @deprecated Use getNavItemsForRoles instead */
export const MAIN_NAV_ITEMS = ADMIN_NAV_ITEMS;

export function getNavItemsForRoles(userRoles: string[]): NavItem[] {
  const isAdmin = userRoles.some((r) => r.toLowerCase() === 'admin');
  const source = isAdmin ? ADMIN_NAV_ITEMS : EMPLOYEE_NAV_ITEMS;

  return source.filter((item) => {
    if (!item.roles?.length) {
      return true;
    }
    return item.roles.some((role) =>
      userRoles.some((r) => r.toLowerCase() === role.toLowerCase()),
    );
  });
}
