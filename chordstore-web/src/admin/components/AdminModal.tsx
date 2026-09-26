import { useEffect, type ReactNode } from 'react';

interface Props {
  title: string;
  onClose: () => void;
  children: ReactNode;
  width?: number;
}

export default function AdminModal({ title, onClose, children, width = 520 }: Props) {
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => e.key === 'Escape' && onClose();
    document.addEventListener('keydown', onKey);
    return () => document.removeEventListener('keydown', onKey);
  }, [onClose]);

  return (
    <div className="adm-overlay" onMouseDown={(e) => e.target === e.currentTarget && onClose()}>
      <div role="dialog" aria-modal="true" aria-label={title} className="adm-modal" style={{ maxWidth: width }}>
        <div className="adm-modal__header">
          <h2>{title}</h2>
          <button type="button" className="adm-modal__close" onClick={onClose} aria-label="Fechar">
            ×
          </button>
        </div>
        <div className="adm-modal__body">{children}</div>
      </div>
    </div>
  );
}
