import { useUI } from '../../context/UIContext';
import type { Instrument } from '../../types';
import AboutModal from './AboutModal';
import CartModal from './CartModal';
import LocationModal from './LocationModal';
import LoginModal from './LoginModal';
import ProductModal from './ProductModal';
import PromoModal from './PromoModal';

export default function ModalRoot({ items }: { items: Instrument[] }) {
  const { modal } = useUI();
  if (!modal) return null;

  switch (modal.type) {
    case 'cart':
      return <CartModal />;
    case 'login':
      return <LoginModal />;
    case 'location':
      return <LocationModal />;
    case 'about':
      return <AboutModal />;
    case 'promo': {
      const promo = items.find((i) => i.section === 'promocao');
      return promo ? <PromoModal instrument={promo} /> : null;
    }
    case 'product': {
      const item = items.find((i) => i.slug === modal.slug);
      return item ? <ProductModal instrument={item} /> : null;
    }
  }
}
