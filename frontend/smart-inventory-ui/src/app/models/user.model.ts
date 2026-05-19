export interface UserDto {
  id: number;
  username: string;
  role: string;
}

export interface CreateUserDto {
  username: string;
  password: string;
  role: string;
}

export interface UpdateUserDto {
  username: string;
  role: string;
  password?: string | null;
}

export interface UserQueryParams {
  pageNumber: number;
  pageSize: number;
  search?: string;
}

export const USER_ROLES = ['Admin', 'Employee'] as const;
export type UserRole = (typeof USER_ROLES)[number];
