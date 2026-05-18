export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
}

export interface AuthUser {
  id: string;
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
