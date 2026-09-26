const planos = [
  {
    title: 'Pix',
    highlight: '5% de desconto',
    description: 'Pagamento à vista com aprovação imediata. O desconto é aplicado automaticamente no checkout.',
  },
  {
    title: 'Cartão de crédito',
    highlight: 'Em até 12x',
    description: 'Parcele suas compras em até 12 vezes. Consulte juros e condições da sua operadora no checkout.',
  },
  {
    title: 'Cartão de débito',
    highlight: 'À vista',
    description: 'Pagamento à vista, debitado diretamente da sua conta no momento da compra.',
  },
  {
    title: 'Boleto bancário',
    highlight: 'Compensação em até 2 dias úteis',
    description: 'Gere o boleto no checkout e pague em qualquer banco, lotérica ou internet banking.',
  },
  {
    title: 'Dinheiro',
    highlight: 'Retirada em loja',
    description: 'Reserve online e finalize o pagamento em espécie na retirada em uma de nossas lojas físicas.',
  },
];

export default function PlanosPage() {
  return (
    <section className="planos">
      <div className="planos__intro">
        <h1 className="pill-title planos__title">FORMAS DE PAGAMENTO E PARCELAMENTO</h1>
        <p className="planos__subtitle">
          Escolha a forma de pagamento que funciona melhor pra você. Todas essas opções estão disponíveis na
          hora de finalizar sua compra no carrinho.
        </p>
      </div>

      <div className="planos__grid">
        {planos.map((p) => (
          <article key={p.title} className="planos__card">
            <h2>{p.title}</h2>
            <p className="planos__highlight">{p.highlight}</p>
            <p>{p.description}</p>
          </article>
        ))}
      </div>

      <p className="planos__note">
        As condições de parcelamento podem variar conforme a operadora do seu cartão. Em caso de dúvidas, fale
        com a gente pelo WhatsApp ou pelo e-mail <a href="mailto:contato@chordstore.com">contato@chordstore.com</a>.
      </p>
    </section>
  );
}
