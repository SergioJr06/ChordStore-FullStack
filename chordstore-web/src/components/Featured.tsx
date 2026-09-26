import { useCart } from '../context/CartContext';
import { useUI } from '../context/UIContext';
import type { Instrument } from '../types';
import { money } from '../utils/format';

interface Props {
  exclusive?: Instrument;
  promo?: Instrument;
}

export default function Featured({ exclusive, promo }: Props) {
  const { open } = useUI();
  const { add } = useCart();

  const discount =
    promo?.oldPrice && promo.price ? Math.round((1 - promo.price / promo.oldPrice) * 100) : null;

  return (
    <section className="exclusivos">
      <div className="exclusivos__grid">
        {exclusive && (
          <article className="col col--excl">
            <h2 className="col__title col__title--gold">EXCLUSIVOS</h2>
            <div className="col__img">
              <img src={exclusive.imageUrl} alt={exclusive.name} />
            </div>
            <h3 className="excl__name">{exclusive.name}</h3>
            <button className="pill-solid pill-solid--excl" onClick={() => open({ type: 'product', slug: exclusive.slug })}>
              Ver Detalhes
            </button>
          </article>
        )}

        {promo && (
          <article className="col col--promo">
            <h2 className="col__title col__title--red">PROMOÇÃO</h2>
            <div className="col__img">
              <img src={promo.imageUrl} alt={promo.name} />
              {discount != null && (
                <span className="badge30">
                  <img src="/images/badge-30.svg" alt="" />
                  <span>{discount}%</span>
                </span>
              )}
            </div>
            {promo.price != null && <p className="promo__price">{money(promo.price)}</p>}
            <button
              className="pill-solid pill-solid--promo"
              onClick={() => {
                add(promo);
                open({ type: 'promo' });
              }}
            >
              Compre Agora
            </button>
          </article>
        )}
      </div>
    </section>
  );
}
