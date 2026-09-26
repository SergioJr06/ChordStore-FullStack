import { useEffect, useState, type FormEvent } from 'react';
import AdminModal from '../components/AdminModal';
import { AdminApiError } from '../api/adminClient';
import { listCustomers } from '../api/customers';
import { listProducts } from '../api/products';
import { createSale, listSales, updateSaleStatus } from '../api/sales';
import type { AdminProduct, Customer, PaymentMethod, Sale, SaleStatus } from '../types';
import { money } from '../../utils/format';

const paymentLabels: Record<PaymentMethod, string> = {
  Pix: 'Pix',
  CartaoCredito: 'Cartão de crédito',
  CartaoDebito: 'Cartão de débito',
  Boleto: 'Boleto',
  Dinheiro: 'Dinheiro',
};

interface CartLine {
  instrumentId: number;
  name: string;
  unitPrice: number;
  quantity: number;
}

export default function AdminSalesPage() {
  const [sales, setSales] = useState<Sale[]>([]);
  const [products, setProducts] = useState<AdminProduct[]>([]);
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [statusFilter, setStatusFilter] = useState<SaleStatus | ''>('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [showForm, setShowForm] = useState(false);
  const [customerId, setCustomerId] = useState('');
  const [walkInName, setWalkInName] = useState('');
  const [productToAdd, setProductToAdd] = useState('');
  const [cart, setCart] = useState<CartLine[]>([]);
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>('Pix');
  const [installments, setInstallments] = useState(1);
  const [formError, setFormError] = useState('');
  const [busy, setBusy] = useState(false);

  const load = (status?: SaleStatus) => {
    setLoading(true);
    listSales(status || undefined)
      .then(setSales)
      .catch(() => setError('Não foi possível carregar as vendas.'))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    load();
    listProducts().then(setProducts).catch(() => {});
    listCustomers().then(setCustomers).catch(() => {});
  }, []);

  const filterByStatus = (status: SaleStatus | '') => {
    setStatusFilter(status);
    load(status || undefined);
  };

  const openCreate = () => {
    setCustomerId('');
    setWalkInName('');
    setProductToAdd('');
    setCart([]);
    setPaymentMethod('Pix');
    setInstallments(1);
    setFormError('');
    setShowForm(true);
  };

  const addProductToCart = () => {
    if (!productToAdd) return;
    const product = products.find((p) => p.id === Number(productToAdd));
    if (!product || product.price == null) return;

    setCart((prev) => {
      const existing = prev.find((l) => l.instrumentId === product.id);
      if (existing) {
        return prev.map((l) => (l.instrumentId === product.id ? { ...l, quantity: l.quantity + 1 } : l));
      }
      return [...prev, { instrumentId: product.id, name: product.name, unitPrice: product.price!, quantity: 1 }];
    });
  };

  const updateQty = (id: number, qty: number) => {
    setCart((prev) => prev.map((l) => (l.instrumentId === id ? { ...l, quantity: Math.max(1, qty) } : l)));
  };

  const removeLine = (id: number) => setCart((prev) => prev.filter((l) => l.instrumentId !== id));

  const cartTotal = cart.reduce((sum, l) => sum + l.unitPrice * l.quantity, 0);

  const submit = async (e: FormEvent) => {
    e.preventDefault();
    setFormError('');

    if (cart.length === 0) {
      setFormError('Adicione pelo menos um produto à venda.');
      return;
    }

    const selectedCustomer = customerId ? customers.find((c) => c.id === Number(customerId)) : null;
    const customerName = selectedCustomer?.name ?? walkInName;

    if (!customerName) {
      setFormError('Selecione um cliente cadastrado ou informe o nome do cliente avulso.');
      return;
    }

    setBusy(true);
    try {
      await createSale({
        customerId: selectedCustomer?.id ?? null,
        customerName,
        customerEmail: selectedCustomer?.email ?? null,
        customerPhone: selectedCustomer?.phone ?? null,
        paymentMethod,
        installments: paymentMethod === 'CartaoCredito' ? installments : 1,
        items: cart.map((l) => ({ instrumentId: l.instrumentId, quantity: l.quantity })),
      });
      setShowForm(false);
      load(statusFilter || undefined);
      listProducts().then(setProducts).catch(() => {}); // atualiza estoque na tela
    } catch (err) {
      setFormError(err instanceof AdminApiError ? err.message : 'Não foi possível registrar a venda.');
    } finally {
      setBusy(false);
    }
  };

  const changeStatus = async (sale: Sale, status: SaleStatus) => {
    try {
      await updateSaleStatus(sale.id, status);
      load(statusFilter || undefined);
    } catch (err) {
      alert(err instanceof AdminApiError ? err.message : 'Não foi possível atualizar o status.');
    }
  };

  return (
    <div>
      <header className="adm-page-header">
        <div>
          <h1>Vendas</h1>
          <p>Vendas feitas na loja (site) e lançadas manualmente</p>
        </div>
        <button className="adm-btn adm-btn--primary" onClick={openCreate}>
          + Lançar venda
        </button>
      </header>

      <div className="adm-search">
        <select value={statusFilter} onChange={(e) => filterByStatus(e.target.value as SaleStatus | '')}>
          <option value="">Todos os status</option>
          <option value="Pendente">Pendente</option>
          <option value="Pago">Pago</option>
          <option value="Cancelado">Cancelado</option>
        </select>
      </div>

      {error && <p className="adm-form-error">{error}</p>}

      {loading ? (
        <p className="adm-loading">Carregando...</p>
      ) : sales.length === 0 ? (
        <p className="adm-empty">Nenhuma venda encontrada.</p>
      ) : (
        <table className="adm-table">
          <thead>
            <tr>
              <th>#</th>
              <th>Cliente</th>
              <th>Origem</th>
              <th>Data</th>
              <th>Pagamento</th>
              <th>Total</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {sales.map((s) => (
              <tr key={s.id}>
                <td>#{s.id}</td>
                <td>{s.customerName}</td>
                <td>{s.origin === 'loja' ? 'Site' : 'Admin'}</td>
                <td>{new Date(s.createdAt).toLocaleString('pt-BR')}</td>
                <td>{paymentLabels[s.paymentMethod]}</td>
                <td>{money(s.total)}</td>
                <td>
                  <select value={s.status} onChange={(e) => changeStatus(s, e.target.value as SaleStatus)}>
                    <option value="Pendente">Pendente</option>
                    <option value="Pago">Pago</option>
                    <option value="Cancelado">Cancelado</option>
                  </select>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {showForm && (
        <AdminModal title="Lançar venda" onClose={() => setShowForm(false)} width={640}>
          <form className="adm-form" onSubmit={submit}>
            <label htmlFor="sale-customer">Cliente cadastrado</label>
            <select id="sale-customer" value={customerId} onChange={(e) => setCustomerId(e.target.value)}>
              <option value="">Cliente avulso (sem cadastro)</option>
              {customers.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>

            {!customerId && (
              <>
                <label htmlFor="sale-walkin">Nome do cliente avulso</label>
                <input id="sale-walkin" value={walkInName} onChange={(e) => setWalkInName(e.target.value)} />
              </>
            )}

            <label htmlFor="sale-product">Adicionar produto</label>
            <div className="adm-form__inline">
              <select id="sale-product" value={productToAdd} onChange={(e) => setProductToAdd(e.target.value)}>
                <option value="">Selecione um produto...</option>
                {products
                  .filter((p) => p.price != null && p.stockQuantity > 0)
                  .map((p) => (
                    <option key={p.id} value={p.id}>
                      {p.name} — {money(p.price!)} (estoque: {p.stockQuantity})
                    </option>
                  ))}
              </select>
              <button type="button" className="adm-btn adm-btn--ghost" onClick={addProductToCart}>
                Adicionar
              </button>
            </div>

            {cart.length > 0 && (
              <table className="adm-table adm-table--compact">
                <thead>
                  <tr>
                    <th>Produto</th>
                    <th>Qtd.</th>
                    <th>Subtotal</th>
                    <th></th>
                  </tr>
                </thead>
                <tbody>
                  {cart.map((l) => (
                    <tr key={l.instrumentId}>
                      <td>{l.name}</td>
                      <td>
                        <input
                          type="number"
                          min={1}
                          value={l.quantity}
                          onChange={(e) => updateQty(l.instrumentId, Number(e.target.value))}
                          className="adm-qty-input"
                        />
                      </td>
                      <td>{money(l.unitPrice * l.quantity)}</td>
                      <td>
                        <button type="button" className="adm-link adm-link--danger" onClick={() => removeLine(l.instrumentId)}>
                          Remover
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}

            <div className="adm-form__grid">
              <div>
                <label htmlFor="sale-payment">Forma de pagamento</label>
                <select
                  id="sale-payment"
                  value={paymentMethod}
                  onChange={(e) => setPaymentMethod(e.target.value as PaymentMethod)}
                >
                  {Object.entries(paymentLabels).map(([value, label]) => (
                    <option key={value} value={value}>
                      {label}
                    </option>
                  ))}
                </select>
              </div>
              {paymentMethod === 'CartaoCredito' && (
                <div>
                  <label htmlFor="sale-installments">Parcelas</label>
                  <input
                    id="sale-installments"
                    type="number"
                    min={1}
                    max={12}
                    value={installments}
                    onChange={(e) => setInstallments(Number(e.target.value))}
                  />
                </div>
              )}
            </div>

            <p className="adm-total">Total: {money(cartTotal)}</p>

            {formError && <p className="adm-form-error">{formError}</p>}

            <div className="adm-form__actions">
              <button type="button" className="adm-btn adm-btn--ghost" onClick={() => setShowForm(false)}>
                Cancelar
              </button>
              <button type="submit" className="adm-btn adm-btn--primary" disabled={busy}>
                {busy ? 'Registrando...' : 'Registrar venda'}
              </button>
            </div>
          </form>
        </AdminModal>
      )}
    </div>
  );
}
