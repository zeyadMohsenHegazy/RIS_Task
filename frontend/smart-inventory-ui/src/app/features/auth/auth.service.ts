import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AuthUser,
  JwtPayload,
  LoginRequest,
  LoginResponse,
} from '../../models/auth.model';

const TOKEN_STORAGE_KEY = 'sim_access_token';
const ROLE_CLAIM =
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly token = signal<string | null>(null);
  private readonly user = signal<AuthUser | null>(null);
  private readonly loading = signal(false);

  readonly isLoading = this.loading.asReadonly();
  readonly currentUser = this.user.asReadonly();
  readonly roles = computed(() => this.user()?.roles ?? []);
  readonly isAdmin = computed(() => this.hasRole('Admin'));
  readonly isEmployee = computed(() => this.hasRole('Employee'));
  readonly isAuthenticated = computed(
    () => !!this.token() && !this.isTokenExpired(this.token()!),
  );

  constructor() {
    this.restoreSession();
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    this.loading.set(true);
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/login`, credentials)
      .pipe(
        tap({
          next: (response) => this.setSession(response.token),
          finalize: () => this.loading.set(false),
        }),
      );
  }

  logout(redirect = true): void {
    this.clearSession();
    if (redirect) {
      void this.router.navigate(['/login']);
    }
  }

  getToken(): string | null {
    return this.token();
  }

  hasRole(role: string): boolean {
    return this.roles().some((r) => r.toLowerCase() === role.toLowerCase());
  }

  hasAnyRole(...requiredRoles: string[]): boolean {
    return requiredRoles.some((role) => this.hasRole(role));
  }

  private setSession(accessToken: string): void {
    const payload = this.decodeToken(accessToken);
    if (!payload) {
      this.clearSession();
      return;
    }

    const roles = this.extractRoles(payload);
    const email =
      payload.email ?? payload.unique_name ?? payload.sub ?? 'unknown';

    localStorage.setItem(TOKEN_STORAGE_KEY, accessToken);
    this.token.set(accessToken);
    this.user.set({
      id: payload.sub ?? email,
      email: String(email),
      roles,
    });
  }

  private restoreSession(): void {
    const stored = localStorage.getItem(TOKEN_STORAGE_KEY);
    if (!stored) {
      return;
    }

    if (this.isTokenExpired(stored)) {
      this.clearSession();
      return;
    }

    this.setSession(stored);
  }

  private clearSession(): void {
    localStorage.removeItem(TOKEN_STORAGE_KEY);
    this.token.set(null);
    this.user.set(null);
  }

  private decodeToken(accessToken: string): JwtPayload | null {
    try {
      return jwtDecode<JwtPayload>(accessToken);
    } catch {
      return null;
    }
  }

  private extractRoles(payload: JwtPayload): string[] {
    const raw = payload.roles ?? payload.role ?? payload[ROLE_CLAIM];
    if (!raw) {
      return [];
    }
    return (Array.isArray(raw) ? raw : [raw]).map((role) => String(role));
  }

  private isTokenExpired(accessToken: string): boolean {
    const payload = this.decodeToken(accessToken);
    if (!payload?.exp) {
      return false;
    }
    return payload.exp * 1000 <= Date.now();
  }
}
