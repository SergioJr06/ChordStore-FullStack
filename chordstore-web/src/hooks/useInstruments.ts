import { useEffect, useState } from 'react';
import { getInstruments } from '../api/instruments';
import { fallbackInstruments } from '../data/fallback';
import type { Instrument } from '../types';

export function useInstruments() {
  const [items, setItems] = useState<Instrument[]>(fallbackInstruments);
  const [online, setOnline] = useState<boolean | null>(null);

  useEffect(() => {
    let alive = true;
    getInstruments()
      .then((data) => {
        if (!alive) return;
        if (data.length) setItems(data);
        setOnline(true);
      })
      .catch((err) => {
        console.warn('API indisponível, usando dados locais do Figma:', err);
        if (alive) setOnline(false);
      });
    return () => {
      alive = false;
    };
  }, []);

  return { items, online };
}
