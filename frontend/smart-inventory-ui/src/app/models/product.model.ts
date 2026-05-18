export interface ProductDto {
  id: number;
  name: string;
  sku: string;
  price: number;
  quantity: number;
  warehouseId: number;
  warehouseName?: string;
  createdAt: string;
}
