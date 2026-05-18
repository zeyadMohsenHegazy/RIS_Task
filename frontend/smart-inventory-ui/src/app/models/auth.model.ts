/** Matches Back-End `LoginRequestDto` (Username, Password) — sent as camelCase JSON. */
export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token?: string;
  /** PascalCase fallback when API serializes with default naming. */
  Token?: string;
}

export interface AuthUser {
  id: string;
  username: string;
  email: string;
  roles: string[];
}

export interface JwtPayload {
  sub?: string;
  email?: string;
  unique_name?: string;
  role?: string | string[];
  roles?: string | string[];
  exp?: number;
  [claim: string]: unknown;
}
