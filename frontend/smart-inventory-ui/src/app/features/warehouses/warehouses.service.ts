import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { WarehouseDto } from '../../models/warehouse.model';

@Injectable({ providedIn: 'root' })
export class WarehousesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/warehouses`;

  getAll(): Observable<WarehouseDto[]> {
    return this.http.get<WarehouseDto[]>(this.baseUrl);
  }
}
