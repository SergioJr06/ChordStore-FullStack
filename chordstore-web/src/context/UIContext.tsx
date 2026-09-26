import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from 'react';
import type { ModalState } from '../types';

interface UICtx {
  modal: ModalState;
  open: (m: NonNullable<ModalState>) => void;
  close: () => void;
}

const Ctx = createContext<UICtx | null>(null);

export function UIProvider({ children }: { children: ReactNode }) {
  const [modal, setModal] = useState<ModalState>(null);
  const open = useCallback((m: NonNullable<ModalState>) => setModal(m), []);
  const close = useCallback(() => setModal(null), []);
  const value = useMemo(() => ({ modal, open, close }), [modal, open, close]);
  return <Ctx.Provider value={value}>{children}</Ctx.Provider>;
}

export function useUI() {
  const ctx = useContext(Ctx);
  if (!ctx) throw new Error('useUI fora do UIProvider');
  return ctx;
}
