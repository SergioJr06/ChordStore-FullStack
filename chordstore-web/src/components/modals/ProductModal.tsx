import { useState, type CSSProperties } from 'react';
import { defaultTheme, productThemes } from '../../data/themes';
import { useCart } from '../../context/CartContext';
import { useUI } from '../../context/UIContext';
import type { Instrument } from '../../types';
import { money, moneyExact } from '../../utils/format';
import Modal from './Modal';

export default function ProductModal({ instrument }: { instrument: Instrument }) {
  const { open, close } = useUI();
  const { add } = useCart();
  const [added, setAdded] = useState(false);

  const theme = productThemes[instrument.slug] ?? defaultTheme;
  const style = {
    '--m-bg': theme.bg,
    '--m-title': theme.title,
    '--m-price': theme.price,
    '--title-size': `${theme.titleSize}px`,
    '--price-size': `${theme.priceSize}px`,
  } as CSSProperties;

  const gallery = instrument.galleryUrls.length ? instrument.galleryUrls : [instrument.imageUrl];
  const { price, installments } = instrument;
  const priceLabel =
    price == null
      ? 'Preço sob consulta'
      : `${money(price)} ou ${installments}x de ${moneyExact(price / installments)}`;

  const buy = () => {
    add(instrument);
    open({ type: 'cart' });
  };
  const addOnly = () => {
    add(instrument);
    setAdded(true);
  };

  return (
    <Modal label={instrument.name} onClose={close} exitIcon={theme.exit} exitWidth={47} className="modal--product" style={style}>
      <div className="product__gallery">
        {gallery.map((src) => (
          <img key={src} src={src} alt={instrument.name} />
        ))}
      </div>

      <h2 className="product__title">{instrument.name}</h2>

      <div className="product__buy">
        <p className="product__price">{priceLabel}</p>
        <button className="buy-btn" onClick={buy} disabled={price == null}>
          Comprar
        </button>
        <button className="cart-btn" onClick={addOnly} disabled={price == null} aria-label="Adicionar ao carrinho">
          <img className="cart-btn__circle" src="/images/btn-cart-circle.svg" alt="" />
          <img className="cart-btn__icon" src="/images/btn-cart-icon.svg" alt="" />
        </button>
      </div>
      <p className="product__added" role="status">
        {added ? 'Adicionado ao carrinho.' : ''}
      </p>
    </Modal>
  );
}
