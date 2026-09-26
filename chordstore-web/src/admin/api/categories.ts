import { adminApi } from './adminClient';
import type { Category } from '../types';

export interface CategoryInput {
  name: string;
  description: string | null;
}

export const listCategories = () => adminApi<Category[]>('/api/admin/categories');

export const createCategory = (input: CategoryInput) =>
  adminApi<Category>('/api/admin/categories', { method: 'POST', body: JSON.stringify(input) });

export const updateCategory = (id: number, input: CategoryInput) =>
  adminApi<Category>(`/api/admin/categories/${id}`, { method: 'PUT', body: JSON.stringify(input) });

export const deleteCategory = (id: number) =>
  adminApi<void>(`/api/admin/categories/${id}`, { method: 'DELETE' });
