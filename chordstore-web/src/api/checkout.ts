import { api } from './client';
import type { PaymentMethod } from '../types';

export interface CheckoutItem {
  instrumentId: number;
  quantity: number;
}

export interface CheckoutPayload {
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  customerAddress?: string;
  paymentMethod: PaymentMethod;
  installments: number;
  items: CheckoutItem[];
}

export interface CheckoutResult {
  orderId: number;
  total: number;
  createdAt: string;
}

export const checkout = (payload: CheckoutPayload) =>
  api<CheckoutResult>('/api/checkout', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
