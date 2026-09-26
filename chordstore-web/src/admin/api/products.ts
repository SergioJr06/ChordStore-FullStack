import { adminApi } from './adminClient';
import type { AdminProduct, AdminProductInput } from '../types';

export const listProducts = (search?: string) =>
  adminApi<AdminProduct[]>(`/api/admin/products${search ? `?search=${encodeURIComponent(search)}` : ''}`);

export const createProduct = (input: AdminProductInput) =>
  adminApi<AdminProduct>('/api/admin/products', { method: 'POST', body: JSON.stringify(input) });

export const updateProduct = (id: number, input: AdminProductInput) =>
  adminApi<AdminProduct>(`/api/admin/products/${id}`, { method: 'PUT', body: JSON.stringify(input) });

export const deleteProduct = (id: number) =>
  adminApi<void>(`/api/admin/products/${id}`, { method: 'DELETE' });
