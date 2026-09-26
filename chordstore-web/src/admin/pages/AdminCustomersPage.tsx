import { useEffect, useState, type FormEvent } from 'react';
import AdminModal from '../components/AdminModal';
import { AdminApiError } from '../api/adminClient';
import { createCustomer, deleteCustomer, listCustomers, updateCustomer } from '../api/customers';
import type { Customer } from '../types';

const emptyForm = { name: '', email: '', phone: '', document: '', address: '' };

export default function AdminCustomersPage() {
  const [items, setItems] = useState<Customer[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [search, setSearch] = useState('');
  const [editing, setEditing] = useState<Customer | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState(emptyForm);
  const [formError, setFormError] = useState('');
  const [busy, setBusy] = useState(false);

  const load = (term?: string) => {
    setLoading(true);
    listCustomers(term)
      .then(setItems)
      .catch(() => setError('Não foi possível carregar os clientes.'))
      .finally(() => setLoading(false));
  };

  useEffect(() => load(), []);

  const runSearch = (e: FormEvent) => {
    e.preventDefault();
    load(search);
  };

  const openCreate = () => {
    setEditing(null);
    setForm(emptyForm);
    setFormError('');
    setShowForm(true);
  };

  const openEdit = (c: Customer) => {
    setEditing(c);
    setForm({
      name: c.name,
      email: c.email ?? '',
      phone: c.phone ?? '',
      document: c.document ?? '',
      address: c.address ?? '',
    });
    setFormError('');
    setShowForm(true);
  };

  const submit = async (e: FormEvent) => {
    e.preventDefault();
    setFormError('');
    setBusy(true);
    try {
      const payload = {
        name: form.name,
        email: form.email || null,
        phone: form.phone || null,
        document: form.document || null,
        address: form.address || null,
      };
      if (editing) await updateCustomer(editing.id, payload);
      else await createCustomer(payload);
      setShowForm(false);
      load(search);
    } catch (err) {
      setFormError(err instanceof AdminApiError ? err.message : 'Não foi possível salvar o cliente.');
    } finally {
      setBusy(false);
    }
  };

  const remove = async (c: Customer) => {
    if (!confirm(`Excluir o cliente "${c.name}"?`)) return;
    try {
      await deleteCustomer(c.id);
      load(search);
    } catch (err) {
      alert(err instanceof AdminApiError ? err.message : 'Não foi possível excluir o cliente.');
    }
  };

  return (
    <div>
      <header className="adm-page-header">
        <div>
          <h1>Clientes</h1>
          <p>Cadastro de clientes da loja</p>
        </div>
        <button className="adm-btn adm-btn--primary" onClick={openCreate}>
          + Novo cliente
        </button>
      </header>

      <form className="adm-search" onSubmit={runSearch}>
        <input
          placeholder="Buscar por nome, e-mail ou CPF..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <button type="submit" className="adm-btn adm-btn--ghost">
          Buscar
        </button>
      </form>

      {error && <p className="adm-form-error">{error}</p>}

      {loading ? (
        <p className="adm-loading">Carregando...</p>
      ) : items.length === 0 ? (
        <p className="adm-empty">Nenhum cliente encontrado.</p>
      ) : (
        <table className="adm-table">
          <thead>
            <tr>
              <th>Nome</th>
              <th>Contato</th>
              <th>Documento</th>
              <th>Compras</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {items.map((c) => (
              <tr key={c.id}>
                <td>{c.name}</td>
                <td>
                  {c.email ?? '—'}
                  {c.phone ? ` · ${c.phone}` : ''}
                </td>
                <td>{c.document ?? '—'}</td>
                <td>{c.salesCount}</td>
                <td className="adm-table__actions">
                  <button className="adm-link" onClick={() => openEdit(c)}>
                    Editar
                  </button>
                  <button className="adm-link adm-link--danger" onClick={() => remove(c)}>
                    Excluir
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {showForm && (
        <AdminModal title={editing ? 'Editar cliente' : 'Novo cliente'} onClose={() => setShowForm(false)}>
          <form className="adm-form" onSubmit={submit}>
            <label htmlFor="cus-name">Nome</label>
            <input
              id="cus-name"
              value={form.name}
              onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
              required
            />

            <label htmlFor="cus-email">E-mail</label>
            <input
              id="cus-email"
              type="email"
              value={form.email}
              onChange={(e) => setForm((f) => ({ ...f, email: e.target.value }))}
            />

            <label htmlFor="cus-phone">Telefone</label>
            <input
              id="cus-phone"
              value={form.phone}
              onChange={(e) => setForm((f) => ({ ...f, phone: e.target.value }))}
            />

            <label htmlFor="cus-doc">CPF</label>
            <input
              id="cus-doc"
              value={form.document}
              onChange={(e) => setForm((f) => ({ ...f, document: e.target.value }))}
            />

            <label htmlFor="cus-address">Endereço</label>
            <input
              id="cus-address"
              value={form.address}
              onChange={(e) => setForm((f) => ({ ...f, address: e.target.value }))}
            />

            {formError && <p className="adm-form-error">{formError}</p>}

            <div className="adm-form__actions">
              <button type="button" className="adm-btn adm-btn--ghost" onClick={() => setShowForm(false)}>
                Cancelar
              </button>
              <button type="submit" className="adm-btn adm-btn--primary" disabled={busy}>
                {busy ? 'Salvando...' : 'Salvar'}
              </button>
            </div>
          </form>
        </AdminModal>
      )}
    </div>
  );
}
