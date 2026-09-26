import { useState, type FormEvent } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { useUI } from '../context/UIContext';
import type { Instrument } from '../types'; // NOVO

interface Props {
  query: string;
  onQuery: (q: string) => void;
  items: Instrument[]; // NOVO: lista de instrumentos, usada para gerar as sugestões
}

function PinIcon() {
  return (
    <span className="pin" aria-hidden="true">
      <span className="pin__a">
        <img src="/images/icon-pin.svg" alt="" />
      </span>
      <span className="pin__b">
        <img src="/images/icon-pin-dot.svg" alt="" />
      </span>
    </span>
  );
}

// NOVO: conteúdo do letreiro extraído para não repetir o JSX na duplicata do loop
function MarqueeContent() {
  return (
    <span>
      <img src="/images/topbar-left.svg" alt="" />
      ENCONTRE SEU SOM — 35% OFF EM PRODUTOS SELECIONADOS
      <img src="/images/topbar-right.svg" alt="" />
    </span>
  );
}

export default function Header({ query, onQuery, items }: Props) {
  const { open } = useUI();
  const { count } = useCart();
  const navigate = useNavigate();
  const location = useLocation();
  const [showSuggestions, setShowSuggestions] = useState(false); // NOVO

  // NOVO: navega pra Home antes de rolar até a seção, caso a pessoa esteja em outra página (ex.: /planos)
  const goTo = (id: string) => {
    if (location.pathname !== '/') {
      navigate('/');
      setTimeout(() => document.getElementById(id)?.scrollIntoView({ behavior: 'smooth' }), 60);
    } else {
      document.getElementById(id)?.scrollIntoView({ behavior: 'smooth' });
    }
  };

  // NOVO: sugestões de autocompletar a partir do nome dos instrumentos já carregados
  const q = query.trim().toLowerCase();
  const suggestions = q ? items.filter((i) => i.name.toLowerCase().includes(q)).slice(0, 6) : [];

  const onSearch = (e: FormEvent) => {
    e.preventDefault();
    setShowSuggestions(false);
    goTo('produtos');
  };

  // NOVO: ao escolher uma sugestão, preenche a busca e vai direto para os produtos
  const pickSuggestion = (name: string) => {
    onQuery(name);
    setShowSuggestions(false);
    goTo('produtos');
  };

  return (
    <header className="header">
      {/* NOVO: barra vira um letreiro (marquee) - duas cópias lado a lado para o loop ficar contínuo */}
      <div className="topbar">
        <div className="topbar__track">
          <MarqueeContent />
          <MarqueeContent />
        </div>
      </div>
      <img className="header__line" src="/images/header-line.svg" alt="" />

      <div className="header__main">
        <Link className="logo" to="/" aria-label="ChordStore — início">
          <img src="/images/logo.png" alt="ChordStore" />
        </Link>

        <form className="search" role="search" onSubmit={onSearch}>
          <img className="search__icon" src="/images/icon-search.svg" alt="" />
          <input
            type="search"
            value={query}
            onChange={(e) => onQuery(e.target.value)}
            onFocus={() => setShowSuggestions(true)} // NOVO
            onBlur={() => setTimeout(() => setShowSuggestions(false), 150)} // NOVO: delay para o clique na sugestão registrar antes de fechar
            placeholder="Pesquisar"
            aria-label="Pesquisar produtos"
            autoComplete="off"
          />

          {/* NOVO: dropdown de autocompletar */}
          {showSuggestions && q.length > 0 && (
            <ul className="search__suggestions">
              {suggestions.length > 0 ? (
                suggestions.map((s) => (
                  <li key={s.id}>
                    <button type="button" onMouseDown={() => pickSuggestion(s.name)}>
                      <img src={s.imageUrl} alt="" />
                      {s.name}
                    </button>
                  </li>
                ))
              ) : (
                <li className="search__empty">Nenhum produto encontrado.</li>
              )}
            </ul>
          )}
        </form>

        <div className="header__icons">
          <button className="icon-btn" onClick={() => open({ type: 'cart' })} aria-label={`Carrinho (${count} itens)`}>
            <img src="/images/icon-cart.svg" alt="" width={38} height={32} />
            {count > 0 && <span className="badge">{count}</span>}
          </button>
          <button className="icon-btn" onClick={() => open({ type: 'location' })} aria-label="Localização">
            <PinIcon />
          </button>
          <button className="icon-btn" onClick={() => open({ type: 'login' })} aria-label="Entrar na sua conta">
            <img src="/images/icon-user.svg" alt="" width={45} height={37} />
          </button>
        </div>
      </div>

      <nav className="nav" aria-label="Principal">
        <button onClick={() => goTo('top')}>Home</button>
        <button onClick={() => goTo('sobre')}>Sobre</button>
        <button onClick={() => goTo('contato')}>Contato</button>
        <button onClick={() => goTo('produtos')}>Produtos</button>
        <Link to="/planos">Planos</Link>
      </nav>
    </header>
  );
}