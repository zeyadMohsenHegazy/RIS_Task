import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  InventoryHistoryQueryParams,
  InventoryTransactionDto,
} from '../../models/inventory.model';
import { PagedResponse } from '../../models/paged-response.model';

@Injectable({ providedIn: 'root' })
export class InventoryService {
  private readonly http = inject(HttpClient);
  private readonly historyUrl = `${environment.apiUrl}/inventory/history`;

  getHistory(
    params: InventoryHistoryQueryParams,
  ): Observable<PagedResponse<InventoryTransactionDto>> {
    let httpParams = new HttpParams()
      .set('pageNumber', String(params.pageNumber))
      .set('pageSize', String(params.pageSize));

    if (params.search?.trim()) {
      httpParams = httpParams.set('search', params.search.trim());
    }

    if (params.transactionType != null) {
      httpParams = httpParams.set('transactionType', String(params.transactionType));
    }

    return this.http.get<PagedResponse<InventoryTransactionDto>>(this.historyUrl, {
      params: httpParams,
    });
  }
}
