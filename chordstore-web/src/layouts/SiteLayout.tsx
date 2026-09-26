import { useState } from 'react';
import { Outlet } from 'react-router-dom';
import Footer from '../components/Footer';
import Header from '../components/Header';
import WhatsAppButton from '../components/WhatsAppButton';
import ModalRoot from '../components/modals/ModalRoot';
import { useInstruments } from '../hooks/useInstruments';
import type { Instrument } from '../types';

export interface SiteOutletContext {
  items: Instrument[];
  query: string;
}

/**
 * Layout usado pelas páginas públicas (Home, Planos): mantém o mesmo Header,
 * Footer, botão do WhatsApp e modais em todas elas — exatamente como já
 * funcionava antes, só que agora compartilhado entre mais de uma rota.
 */
export default function SiteLayout() {
  const { items } = useInstruments();
  const [query, setQuery] = useState('');

  return (
    <>
      <div id="top" />
      <Header query={query} onQuery={setQuery} items={items} />
      <main>
        <Outlet context={{ items, query } satisfies SiteOutletContext} />
      </main>
      <Footer />
      <WhatsAppButton />
      <ModalRoot items={items} />
    </>
  );
}
