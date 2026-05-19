import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { dataRequestOptions, mutationRequestOptions } from '../../core/http/api-http.options';
import { environment } from '../../../environments/environment';
import { PagedResponse } from '../../models/paged-response.model';
import {
  CreateUserDto,
  UpdateUserDto,
  UserDto,
  UserQueryParams,
} from '../../models/user.model';

@Injectable({ providedIn: 'root' })
export class UsersService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/users`;

  getUsers(params: UserQueryParams): Observable<PagedResponse<UserDto>> {
    let httpParams = new HttpParams()
      .set('pageNumber', String(params.pageNumber))
      .set('pageSize', String(params.pageSize));

    if (params.search?.trim()) {
      httpParams = httpParams.set('search', params.search.trim());
    }

    return this.http.get<PagedResponse<UserDto>>(this.baseUrl, {
      params: httpParams,
      ...dataRequestOptions(),
    });
  }

  getById(id: number): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.baseUrl}/${id}`, dataRequestOptions());
  }

  create(dto: CreateUserDto): Observable<UserDto> {
    return this.http.post<UserDto>(this.baseUrl, dto, mutationRequestOptions());
  }

  update(id: number, dto: UpdateUserDto): Observable<UserDto> {
    return this.http.put<UserDto>(`${this.baseUrl}/${id}`, dto, mutationRequestOptions());
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`, mutationRequestOptions());
  }
}
