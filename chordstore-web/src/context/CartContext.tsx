import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from 'react';
import type { Instrument } from '../types';

export interface CartItem {
  id: number;
  name: string;
  price: number;
  imageUrl: string;
  qty: number;
}

interface CartCtx {
  items: CartItem[];
  count: number;
  total: number;
  add: (i: Instrument) => void;
  remove: (id: number) => void;
  clear: () => void;
}

const KEY = 'chordstore:cart';
const Ctx = createContext<CartCtx | null>(null);

function load(): CartItem[] {
  try {
    return JSON.parse(localStorage.getItem(KEY) ?? '[]') as CartItem[];
  } catch {
    return [];
  }
}

export function CartProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<CartItem[]>(load);

  useEffect(() => {
    try {
      localStorage.setItem(KEY, JSON.stringify(items));
    } catch {
      /* storage indisponível: o carrinho vale só nesta sessão */
    }
  }, [items]);

  const add = useCallback((i: Instrument) => {
    if (i.price == null) return;
    const price = i.price;
    setItems((prev) => {
      const found = prev.find((p) => p.id === i.id);
      if (found) return prev.map((p) => (p.id === i.id ? { ...p, qty: p.qty + 1 } : p));
      return [...prev, { id: i.id, name: i.name, price, imageUrl: i.imageUrl, qty: 1 }];
    });
  }, []);

  const remove = useCallback((id: number) => setItems((prev) => prev.filter((p) => p.id !== id)), []);

  const clear = useCallback(() => setItems([]), []);

  const value = useMemo(
    () => ({
      items,
      add,
      remove,
      clear,
      count: items.reduce((n, i) => n + i.qty, 0),
      total: items.reduce((n, i) => n + i.qty * i.price, 0),
    }),
    [items, add, remove, clear],
  );

  return <Ctx.Provider value={value}>{children}</Ctx.Provider>;
}

export function useCart() {
  const ctx = useContext(Ctx);
  if (!ctx) throw new Error('useCart fora do CartProvider');
  return ctx;
}
