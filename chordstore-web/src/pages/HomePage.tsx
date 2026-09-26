import { useOutletContext } from 'react-router-dom';
import About from '../components/About';
import Featured from '../components/Featured';
import Hero from '../components/Hero';
import NewProducts from '../components/NewProducts';
import type { SiteOutletContext } from '../layouts/SiteLayout';

export default function HomePage() {
  const { items, query } = useOutletContext<SiteOutletContext>();

  const novos = items.filter((i) => i.section === 'novo');
  const exclusive = items.find((i) => i.section === 'exclusivo');
  const promo = items.find((i) => i.section === 'promocao');

  return (
    <>
      <Hero />
      <NewProducts items={novos} query={query} />
      <About />
      <Featured exclusive={exclusive} promo={promo} />
    </>
  );
}
