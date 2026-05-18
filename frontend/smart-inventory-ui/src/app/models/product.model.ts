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

export interface CreateProductDto {
  name: string;
  sku: string;
  price: number;
  quantity: number;
  warehouseId: number;
}

export interface UpdateProductDto {
  name: string;
  sku: string;
  price: number;
  quantity: number;
  warehouseId: number;
}

export interface ProductQueryParams {
  pageNumber: number;
  pageSize: number;
  search?: string;
}
