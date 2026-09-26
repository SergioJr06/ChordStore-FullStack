import { useUI } from '../../context/UIContext';
import Modal from './Modal';

export default function AboutModal() {
  const { close } = useUI();
  return (
    <Modal label="Sobre nós" onClose={close} exitIcon="/images/exit-dark.svg" exitWidth={47} className="modal--about">
      <img className="about__banner" src="/images/logo.png" alt="ChordStore" />
      <p>
        A ChordStore nasceu da vontade de criar uma loja diferente, um lugar onde músicos se sentissem em casa. Começamos pequenos, com um catálogo enxuto, mas sempre com um cuidado obsessivo na escolha de cada instrumento e acessório.
      </p>
      <p>
        De lá pra cá, crescemos, ampliamos nosso estoque e conquistamos clientes de todos os estilos. O que nunca mudou foi nosso compromisso: oferecer o melhor equipamento, com atendimento humano e paixão genuína pela música. Porque pra gente, cada venda é o começo de uma nova canção.
      </p>
      <p className="about__copy">© 2026 ChordStore. Todos os direitos reservados.</p>
    </Modal>
  );
}
