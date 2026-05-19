import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  SKIP_GLOBAL_ERROR_HANDLING,
  SKIP_GLOBAL_LOADER,
} from '../../core/http/http-context.tokens';
import {
  AuthUser,
  JwtPayload,
  LoginRequest,
  LoginResponse,
} from '../../models/auth.model';

const TOKEN_STORAGE_KEY = 'sim_access_token';
const ROLE_CLAIM =
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
const NAME_CLAIM =
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
const NAME_IDENTIFIER_CLAIM =
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';

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
  readonly isAuthenticated = computed(() => this.isLoggedIn());

  constructor() {
    this.restoreSession();
  }

  /** Reliable check for route guards (not only the computed signal). */
  isLoggedIn(): boolean {
    const accessToken = this.token();
    return !!accessToken && !this.isTokenExpired(accessToken);
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    this.loading.set(true);
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/login`, credentials, {
        context: new HttpContext()
          .set(SKIP_GLOBAL_ERROR_HANDLING, true)
          .set(SKIP_GLOBAL_LOADER, true),
      })
      .pipe(
        tap({
          next: (response) => {
            const token = this.extractToken(response);
            if (token) {
              this.setSession(token);
            }
          },
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
    const username = String(
      payload.unique_name ??
        payload[NAME_CLAIM] ??
        payload['name'] ??
        payload.email ??
        payload.sub ??
        'User',
    );
    const email = String(payload.email ?? username);
    const userId = this.extractUserId(payload);

    localStorage.setItem(TOKEN_STORAGE_KEY, accessToken);
    this.token.set(accessToken);
    this.user.set({
      id: userId,
      username,
      email,
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

  private extractToken(response: LoginResponse & { Token?: string }): string | null {
    const token = response.token ?? response.Token;
    return typeof token === 'string' && token.length > 0 ? token : null;
  }

  private extractUserId(payload: JwtPayload): number {
    const raw =
      payload[NAME_IDENTIFIER_CLAIM] ??
      payload.sub ??
      payload['nameid'];

    const parsed = Number(raw);
    if (!Number.isNaN(parsed) && parsed > 0) {
      return parsed;
    }

    return 0;
  }

  private isTokenExpired(accessToken: string): boolean {
    const payload = this.decodeToken(accessToken);
    if (!payload?.exp) {
      return false;
    }
    return payload.exp * 1000 <= Date.now();
  }
}
