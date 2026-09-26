import { api } from './client';

export interface LoginResponse {
  token: string;
}

// Ainda não existe no backend: implemente POST /api/auth/login (ex.: JWT) para o login funcionar.
export const login = (email: string, password: string) =>
  api<LoginResponse>('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify({ email, password }),
  });
