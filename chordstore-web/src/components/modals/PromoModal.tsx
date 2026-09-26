import { useCart } from '../../context/CartContext';
import { useUI } from '../../context/UIContext';
import type { Instrument } from '../../types';
import { money } from '../../utils/format';
import Modal from './Modal';

export default function PromoModal({ instrument }: { instrument: Instrument }) {
  const { open, close } = useUI();
  const { add } = useCart();

  const buy = () => {
    add(instrument);
    open({ type: 'cart' });
  };

  return (
    <Modal label="Promoção" onClose={close} exitIcon="/images/exit-promo.svg" exitWidth={46} className="modal--promo">
      <h2 className="promo__title">PROMOÇÃO</h2>

      <div className="promo__body">
        <img src={instrument.imageUrl} alt={instrument.name} />
        <p>{instrument.description}</p>
      </div>

      {instrument.price != null && (
        <p className="promo__prices">
          {instrument.oldPrice != null && (
            <>
              De: <s>{money(instrument.oldPrice)}</s>{' '}
            </>
          )}
          Por: {money(instrument.price)}
        </p>
      )}

      <button className="promo__buy" onClick={buy} disabled={instrument.price == null}>
        Comprar
      </button>
    </Modal>
  );
}
