import { useCart } from '../context/CartContext';
import { useUI } from '../context/UIContext';
import type { Instrument } from '../types';

interface Props {
  items: Instrument[];
  query: string;
}

export default function NewProducts({ items, query }: Props) {
  const { open } = useUI();
  const { add } = useCart();

  const q = query.trim().toLowerCase();
  const list = q ? items.filter((i) => i.name.toLowerCase().includes(q)) : items;

  const buy = (i: Instrument) => {
    add(i);
    open({ type: 'cart' });
  };

  return (
    <section id="produtos" className="novos">
      <h2>NOVOS PRODUTOS</h2>
      <p className="novos__lead">Novidades, novos sons e tudo para levar sua música ainda mais longe.</p>

      {list.length === 0 ? (
        <p className="novos__empty">Nenhum produto encontrado para “{query}”.</p>
      ) : (
        <ul className="cards">
          {list.map((i) => (
            <li key={i.id} className="card">
              <div className="card__img">
                <img src={i.imageUrl} alt={i.name} />
              </div>
              <h3 className="card__name">{i.name}</h3>
              <button className="pill pill--sm" onClick={() => open({ type: 'product', slug: i.slug })}>
                DETALHES
              </button>
              <button className="pill pill--lg" onClick={() => buy(i)} disabled={i.price == null}>
                COMPRE AGORA
              </button>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
