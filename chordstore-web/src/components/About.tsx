import { useUI } from '../context/UIContext';

export default function About() {
  const { open } = useUI();
  return (
    <section id="sobre" className="sobre">
      <video
        className="sobre__video"
        src="/videos/sobre-bg.mp4"
        poster="/images/sobre-fundo.png"
        autoPlay
        muted
        loop
        playsInline
      />
      <div className="sobre__overlay" />
      <div className="sobre__content">
        <h2>SOBRE</h2>
        <p>Somos uma loja feita por músicos, para músicos. Na ChordStore, cada produto é escolhido pensando em qualidade, confiança e no seu som.</p>
        <button className="pill sobre__more" onClick={() => open({ type: 'about' })}>
          Ler Mais
        </button>
      </div>
    </section>
  );
}
