export type Section = 'novo' | 'exclusivo' | 'promocao';

export type PaymentMethod = 'Pix' | 'CartaoCredito' | 'CartaoDebito' | 'Boleto' | 'Dinheiro';

/** Formato devolvido por GET /api/instruments (camelCase, padrão do ASP.NET Core). */
export interface Instrument {
  id: number;
  slug: string;
  name: string;
  section: Section;
  price: number | null;
  oldPrice: number | null;
  installments: number;
  description: string | null;
  imageUrl: string;
  galleryUrls: string[];
}

export type ModalState =
  | { type: 'cart' }
  | { type: 'login' }
  | { type: 'location' }
  | { type: 'about' }
  | { type: 'promo' }
  | { type: 'product'; slug: string }
  | null;
