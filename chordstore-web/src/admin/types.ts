export interface AdminUser {
  id: number;
  username: string;
  email: string;
}

export interface Category {
  id: number;
  name: string;
  description: string | null;
  productCount: number;
}

export interface Supplier {
  id: number;
  name: string;
  document: string | null;
  phone: string | null;
  email: string | null;
  address: string | null;
  productCount: number;
}

export interface Customer {
  id: number;
  name: string;
  email: string | null;
  phone: string | null;
  document: string | null;
  address: string | null;
  createdAt: string;
  salesCount: number;
}

export interface AdminProduct {
  id: number;
  slug: string;
  name: string;
  section: string;
  price: number | null;
  oldPrice: number | null;
  installments: number;
  description: string | null;
  imageUrl: string;
  galleryUrls: string[];
  brand: string;
  stockQuantity: number;
  categoryId: number | null;
  categoryName: string | null;
  supplierId: number | null;
  supplierName: string | null;
}

export interface AdminProductInput {
  slug: string;
  name: string;
  section: string;
  price: number | null;
  oldPrice: number | null;
  installments: number;
  description: string | null;
  imageUrl: string;
  galleryUrls: string[];
  brand: string;
  stockQuantity: number;
  categoryId: number | null;
  supplierId: number | null;
}

export type SaleStatus = 'Pendente' | 'Pago' | 'Cancelado';
export type PaymentMethod = 'Pix' | 'CartaoCredito' | 'CartaoDebito' | 'Boleto' | 'Dinheiro';

export interface SaleItem {
  instrumentId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
}

export interface Sale {
  id: number;
  customerId: number | null;
  customerName: string;
  customerEmail: string | null;
  customerPhone: string | null;
  createdAt: string;
  status: SaleStatus;
  paymentMethod: PaymentMethod;
  installments: number;
  total: number;
  origin: string;
  items: SaleItem[];
}

export interface SaleInput {
  customerId: number | null;
  customerName: string | null;
  customerEmail: string | null;
  customerPhone: string | null;
  paymentMethod: PaymentMethod;
  installments: number;
  items: { instrumentId: number; quantity: number }[];
}
