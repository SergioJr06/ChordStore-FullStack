import { useEffect, type CSSProperties, type ReactNode } from 'react';

interface Props {
  label: string;
  onClose: () => void;
  exitIcon: string;
  exitWidth: number;
  className?: string;
  style?: CSSProperties;
  children: ReactNode;
}

export default function Modal({ label, onClose, exitIcon, exitWidth, className = '', style, children }: Props) {
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => e.key === 'Escape' && onClose();
    document.addEventListener('keydown', onKey);
    const prev = document.body.style.overflow;
    document.body.style.overflow = 'hidden';
    return () => {
      document.removeEventListener('keydown', onKey);
      document.body.style.overflow = prev;
    };
  }, [onClose]);

  return (
    <div className="overlay" onMouseDown={(e) => e.target === e.currentTarget && onClose()}>
      <div role="dialog" aria-modal="true" aria-label={label} className={`modal ${className}`} style={style}>
        <button className="modal__exit" onClick={onClose} aria-label="Fechar">
          <img src={exitIcon} alt="" style={{ width: exitWidth }} />
        </button>
        {children}
      </div>
    </div>
  );
}
