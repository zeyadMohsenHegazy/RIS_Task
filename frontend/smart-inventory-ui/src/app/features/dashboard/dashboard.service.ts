import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { forkJoin, map, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResponse } from '../../models/paged-response.model';
import { ProductDto } from '../../models/product.model';
import { WarehouseDto } from '../../models/warehouse.model';
import {
  DashboardStats,
  LOW_STOCK_THRESHOLD,
  LowStockProduct,
} from './models/dashboard.model';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  getStats(): Observable<DashboardStats> {
    const countParams = new HttpParams()
      .set('pageNumber', '1')
      .set('pageSize', '1');

    const productsParams = new HttpParams()
      .set('pageNumber', '1')
      .set('pageSize', '100');

    return forkJoin({
      productsPage: this.http.get<PagedResponse<ProductDto>>(
        `${this.baseUrl}/products`,
        { params: countParams },
      ),
      warehouses: this.http.get<WarehouseDto[]>(`${this.baseUrl}/warehouses`),
      transactionsPage: this.http.get<PagedResponse<unknown>>(
        `${this.baseUrl}/inventory/history`,
        { params: countParams },
      ),
      productsList: this.http.get<PagedResponse<ProductDto>>(
        `${this.baseUrl}/products`,
        { params: productsParams },
      ),
    }).pipe(map((result) => this.mapToDashboardStats(result)));
  }

  private mapToDashboardStats(result: {
    productsPage: PagedResponse<ProductDto>;
    warehouses: WarehouseDto[];
    transactionsPage: PagedResponse<unknown>;
    productsList: PagedResponse<ProductDto>;
  }): DashboardStats {
    const lowStockProducts = this.extractLowStockProducts(result.productsList.items);

    return {
      totalProducts: result.productsPage.totalCount,
      totalWarehouses: result.warehouses.length,
      totalInventoryTransactions: result.transactionsPage.totalCount,
      lowStockCount: lowStockProducts.length,
      lowStockProducts,
    };
  }

  private extractLowStockProducts(products: ProductDto[]): LowStockProduct[] {
    return products
      .filter((p) => p.quantity <= LOW_STOCK_THRESHOLD)
      .sort((a, b) => a.quantity - b.quantity)
      .map((p) => ({
        id: p.id,
        name: p.name,
        sku: p.sku,
        quantity: p.quantity,
        warehouseName: p.warehouseName ?? '—',
      }));
  }
}
