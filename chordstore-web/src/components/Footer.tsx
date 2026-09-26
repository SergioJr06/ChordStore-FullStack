export default function Footer() {
  return (
    <>
      <footer id="contato" className="footer">
        <div className="footer__grid">
          <section>
            <h3>SUPORTE</h3>
            <p>Precisa de ajuda?</p>
            <ul>
              <li>Central de Atendimento</li>
              <li>Perguntas Frequentes</li>
              <li>Rastreamento de Pedido</li>
              <li>Trocas e Devoluções</li>
              <li>Política de Privacidade</li>
              <li>Termos e Condições</li>
            </ul>
          </section>

          <section>
            <h3>ENCONTRE-NOS</h3>
            <p>Encontre a ChordStore</p>
            <address>
              Avenida Paulista, 2026
              <br />
              São Paulo — SP, Brasil
            </address>
            <p>
              Segunda a sexta: 9h às 18h
              <br />
              Sábado: 9h às 14h
            </p>
          </section>

          <img className="footer__logo" src="/images/logo.png" alt="ChordStore" />

          <section>
            <h3>ATENDIMENTO</h3>
            <p>Fale com a gente</p>
            <ul>
              <li>
                <a href="https://wa.me/5511900000000" target="_blank" rel="noopener noreferrer">
                  WhatsApp
                </a>
              </li>
              <li>E-mail</li>
              <li>Instagram</li>
            </ul>
            <a href="mailto:contato@chordstore.com">contato@chordstore.com</a>
          </section>

          <section>
            <h3>REDES SOCIAIS</h3>
            <p>SIGA A CHORDSTORE</p>
            <ul>
              <li>Instagram</li>
              <li>Facebook</li>
              <li>YouTube</li>
              <li>TikTok</li>
            </ul>
          </section>
        </div>
      </footer>
      <p className="copy">© 2026 ChordStore. Todos os direitos reservados.</p>
    </>
  );
}
