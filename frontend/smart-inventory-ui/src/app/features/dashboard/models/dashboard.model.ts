/** Products at or below this quantity are considered low stock. */
export const LOW_STOCK_THRESHOLD = 10;

export interface DashboardStats {
  totalProducts: number;
  totalWarehouses: number;
  totalInventoryTransactions: number;
  lowStockCount: number;
  lowStockProducts: LowStockProduct[];
}

export interface LowStockProduct {
  id: number;
  name: string;
  sku: string;
  quantity: number;
  warehouseName: string;
}

export interface DashboardStatCard {
  label: string;
  value: number;
  icon: string;
  accent: 'primary' | 'accent' | 'warn' | 'neutral';
}
