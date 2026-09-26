import { adminApi } from './adminClient';
import type { Supplier } from '../types';

export interface SupplierInput {
  name: string;
  document: string | null;
  phone: string | null;
  email: string | null;
  address: string | null;
}

export const listSuppliers = () => adminApi<Supplier[]>('/api/admin/suppliers');

export const createSupplier = (input: SupplierInput) =>
  adminApi<Supplier>('/api/admin/suppliers', { method: 'POST', body: JSON.stringify(input) });

export const updateSupplier = (id: number, input: SupplierInput) =>
  adminApi<Supplier>(`/api/admin/suppliers/${id}`, { method: 'PUT', body: JSON.stringify(input) });

export const deleteSupplier = (id: number) =>
  adminApi<void>(`/api/admin/suppliers/${id}`, { method: 'DELETE' });
