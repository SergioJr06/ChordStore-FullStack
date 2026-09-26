import { adminApi } from './adminClient';
import type { Customer } from '../types';

export interface CustomerInput {
  name: string;
  email: string | null;
  phone: string | null;
  document: string | null;
  address: string | null;
}

export const listCustomers = (search?: string) =>
  adminApi<Customer[]>(`/api/admin/customers${search ? `?search=${encodeURIComponent(search)}` : ''}`);

export const createCustomer = (input: CustomerInput) =>
  adminApi<Customer>('/api/admin/customers', { method: 'POST', body: JSON.stringify(input) });

export const updateCustomer = (id: number, input: CustomerInput) =>
  adminApi<Customer>(`/api/admin/customers/${id}`, { method: 'PUT', body: JSON.stringify(input) });

export const deleteCustomer = (id: number) =>
  adminApi<void>(`/api/admin/customers/${id}`, { method: 'DELETE' });
