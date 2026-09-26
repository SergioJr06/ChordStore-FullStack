import { adminApi } from './adminClient';
import type { AdminUser } from '../types';

export interface LoginResponse {
  token: string;
  user: AdminUser;
}

export const login = (email: string, password: string) =>
  adminApi<LoginResponse>('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify({ email, password }),
  });

export const fetchMe = () => adminApi<AdminUser>('/api/auth/me');

export const updateAccount = (username: string, email: string) =>
  adminApi<AdminUser>('/api/auth/me', {
    method: 'PUT',
    body: JSON.stringify({ username, email }),
  });

export const changePassword = (currentPassword: string, newPassword: string) =>
  adminApi<void>('/api/auth/change-password', {
    method: 'PUT',
    body: JSON.stringify({ currentPassword, newPassword }),
  });
