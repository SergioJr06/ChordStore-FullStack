import { useUI } from '../../context/UIContext';
import Modal from './Modal';

export default function LocationModal() {
  const { close } = useUI();
  return (
    <Modal label="Localização" onClose={close} exitIcon="/images/exit-localizacao.svg" exitWidth={24} className="modal--location">
      <h2 className="pill-title">LOCALIZAÇÃO</h2>
      <dl>
        <dt>Endereço:</dt>
        <dd>
          Av. Paulista, 1000 — Bela Vista
          <br />
          São Paulo – SP, 01310-100
          <br />
          Brasil
        </dd>
        <dt>E-mail:</dt>
        <dd>chordstore.com.br</dd>
        <dt>Telefone:</dt>
        <dd>(11) 90000-0000</dd>
      </dl>
    </Modal>
  );
}
