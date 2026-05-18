import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { dataRequestOptions } from '../../core/http/api-http.options';
import { environment } from '../../../environments/environment';
import { PagedResponse } from '../../models/paged-response.model';
import {
  CreateProductDto,
  ProductDto,
  ProductQueryParams,
  UpdateProductDto,
} from '../../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductsService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/products`;

  getProducts(params: ProductQueryParams): Observable<PagedResponse<ProductDto>> {
    let httpParams = new HttpParams()
      .set('pageNumber', String(params.pageNumber))
      .set('pageSize', String(params.pageSize));

    if (params.search?.trim()) {
      httpParams = httpParams.set('search', params.search.trim());
    }

    return this.http.get<PagedResponse<ProductDto>>(this.baseUrl, {
      params: httpParams,
      ...dataRequestOptions(),
    });
  }

  getById(id: number): Observable<ProductDto> {
    return this.http.get<ProductDto>(`${this.baseUrl}/${id}`, dataRequestOptions());
  }

  create(dto: CreateProductDto): Observable<ProductDto> {
    return this.http.post<ProductDto>(this.baseUrl, dto);
  }

  update(id: number, dto: UpdateProductDto): Observable<ProductDto> {
    return this.http.put<ProductDto>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
