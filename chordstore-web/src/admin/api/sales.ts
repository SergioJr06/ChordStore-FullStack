import { adminApi } from './adminClient';
import type { Sale, SaleInput, SaleStatus } from '../types';

export const listSales = (status?: SaleStatus) =>
  adminApi<Sale[]>(`/api/admin/sales${status ? `?status=${status}` : ''}`);

export const createSale = (input: SaleInput) =>
  adminApi<Sale>('/api/admin/sales', { method: 'POST', body: JSON.stringify(input) });

export const updateSaleStatus = (id: number, status: SaleStatus) =>
  adminApi<Sale>(`/api/admin/sales/${id}/status`, { method: 'PUT', body: JSON.stringify({ status }) });
