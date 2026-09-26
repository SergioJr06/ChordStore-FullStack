import { useState, type FormEvent } from 'react';
import { checkout } from '../../api/checkout';
import { ApiError } from '../../api/client';
import { useCart } from '../../context/CartContext';
import { useUI } from '../../context/UIContext';
import type { PaymentMethod } from '../../types';
import { money } from '../../utils/format';
import Modal from './Modal';

type Step = 'view' | 'checkout' | 'success';

const paymentLabels: Record<PaymentMethod, string> = {
  Pix: 'Pix',
  CartaoCredito: 'Cartão de crédito',
  CartaoDebito: 'Cartão de débito',
  Boleto: 'Boleto',
  Dinheiro: 'Dinheiro (retirada na loja)',
};

export default function CartModal() {
  const { close } = useUI();
  const { items, remove, total, clear } = useCart();

  const [step, setStep] = useState<Step>('view');
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');
  const [order, setOrder] = useState<{ id: number; total: number } | null>(null);

  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [address, setAddress] = useState('');
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>('Pix');
  const [installments, setInstallments] = useState(1);

  const addMore = () => {
    close();
    document.getElementById('produtos')?.scrollIntoView({ behavior: 'smooth' });
  };

  const submit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setBusy(true);
    try {
      const result = await checkout({
        customerName: name,
        customerEmail: email,
        customerPhone: phone,
        customerAddress: address || undefined,
        paymentMethod,
        installments: paymentMethod === 'CartaoCredito' ? installments : 1,
        items: items.map((i) => ({ instrumentId: i.id, quantity: i.qty })),
      });
      setOrder({ id: result.orderId, total: result.total });
      clear();
      setStep('success');
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Não foi possível concluir o pedido. Tente novamente.');
    } finally {
      setBusy(false);
    }
  };

  // ---------- Passo 3: confirmação ----------
  if (step === 'success' && order) {
    return (
      <Modal label="Pedido confirmado" onClose={close} exitIcon="/images/exit-carrinho.svg" exitWidth={31} className="modal--cart">
        <h2 className="pill-title pill-title--cart">PEDIDO CONFIRMADO!</h2>
        <div className="cart__empty">
          <p className="cart__empty-title">Pedido #{order.id}</p>
          <p className="cart__empty-text">
            Total: {money(order.total)}
            <br />
            Enviamos os detalhes para o seu e-mail. Obrigado por comprar na ChordStore!
          </p>
        </div>
        <button className="pill-btn" onClick={close}>
          FECHAR
        </button>
      </Modal>
    );
  }

  // ---------- Passo 2: dados de finalização ----------
  if (step === 'checkout') {
    return (
      <Modal label="Finalizar compra" onClose={close} exitIcon="/images/exit-carrinho.svg" exitWidth={31} className="modal--cart">
        <h2 className="pill-title pill-title--cart">FINALIZAR COMPRA</h2>

        <form onSubmit={submit} className="checkout-form">
          <label htmlFor="chk-name">Nome completo</label>
          <input id="chk-name" value={name} onChange={(e) => setName(e.target.value)} required />

          <label htmlFor="chk-email">E-mail</label>
          <input id="chk-email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />

          <label htmlFor="chk-phone">Telefone</label>
          <input id="chk-phone" value={phone} onChange={(e) => setPhone(e.target.value)} required />

          <label htmlFor="chk-address">Endereço de entrega (opcional)</label>
          <input id="chk-address" value={address} onChange={(e) => setAddress(e.target.value)} />

          <label htmlFor="chk-payment">Forma de pagamento</label>
          <select
            id="chk-payment"
            value={paymentMethod}
            onChange={(e) => setPaymentMethod(e.target.value as PaymentMethod)}
          >
            {Object.entries(paymentLabels).map(([value, label]) => (
              <option key={value} value={value}>
                {label}
              </option>
            ))}
          </select>

          {paymentMethod === 'CartaoCredito' && (
            <>
              <label htmlFor="chk-installments">Parcelas</label>
              <select
                id="chk-installments"
                value={installments}
                onChange={(e) => setInstallments(Number(e.target.value))}
              >
                {Array.from({ length: 12 }, (_, i) => i + 1).map((n) => (
                  <option key={n} value={n}>
                    {n}x de {money(total / n)}
                  </option>
                ))}
              </select>
            </>
          )}

          <p className="cart__total">Total: {money(total)}</p>

          {error && (
            <p className="form-error" role="alert">
              {error}
            </p>
          )}

          <div className="checkout-form__actions">
            <button type="button" className="pill-btn pill-btn--ghost" onClick={() => setStep('view')} disabled={busy}>
              VOLTAR
            </button>
            <button type="submit" className="pill-btn" disabled={busy}>
              {busy ? 'ENVIANDO...' : 'CONFIRMAR PEDIDO'}
            </button>
          </div>
        </form>
      </Modal>
    );
  }

  // ---------- Passo 1: carrinho ----------
  return (
    <Modal label="Carrinho" onClose={close} exitIcon="/images/exit-carrinho.svg" exitWidth={31} className="modal--cart">
      <h2 className="pill-title pill-title--cart">SEU CARRINHO</h2>

      {items.length === 0 ? (
        <div className="cart__empty">
          <p className="cart__empty-title">SEU CARRINHO ESTÁ VAZIO</p>
          <p className="cart__empty-text">Não há produtos no seu carrinho.</p>
        </div>
      ) : (
        <>
          <ul className="cart__list">
            {items.map((i) => (
              <li key={i.id}>
                <img src={i.imageUrl} alt="" />
                <div>
                  <p className="cart__name">{i.name}</p>
                  <p className="cart__meta">
                    {i.qty} × {money(i.price)}
                  </p>
                </div>
                <button onClick={() => remove(i.id)} aria-label={`Remover ${i.name}`}>
                  Remover
                </button>
              </li>
            ))}
          </ul>
          <p className="cart__total">Total: {money(total)}</p>
        </>
      )}

      <div className="checkout-form__actions">
        <button className="pill-btn pill-btn--ghost" onClick={addMore}>
          ADICIONAR
        </button>
        {items.length > 0 && (
          <button className="pill-btn" onClick={() => setStep('checkout')}>
            FINALIZAR COMPRA
          </button>
        )}
      </div>
    </Modal>
  );
}
