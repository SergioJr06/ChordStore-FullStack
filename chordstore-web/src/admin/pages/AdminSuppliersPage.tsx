import { useEffect, useState, type FormEvent } from 'react';
import AdminModal from '../components/AdminModal';
import { AdminApiError } from '../api/adminClient';
import { createSupplier, deleteSupplier, listSuppliers, updateSupplier } from '../api/suppliers';
import type { Supplier } from '../types';

const emptyForm = { name: '', document: '', phone: '', email: '', address: '' };

export default function AdminSuppliersPage() {
  const [items, setItems] = useState<Supplier[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [editing, setEditing] = useState<Supplier | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState(emptyForm);
  const [formError, setFormError] = useState('');
  const [busy, setBusy] = useState(false);

  const load = () => {
    setLoading(true);
    listSuppliers()
      .then(setItems)
      .catch(() => setError('Não foi possível carregar os fornecedores.'))
      .finally(() => setLoading(false));
  };

  useEffect(load, []);

  const openCreate = () => {
    setEditing(null);
    setForm(emptyForm);
    setFormError('');
    setShowForm(true);
  };

  const openEdit = (s: Supplier) => {
    setEditing(s);
    setForm({
      name: s.name,
      document: s.document ?? '',
      phone: s.phone ?? '',
      email: s.email ?? '',
      address: s.address ?? '',
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
        document: form.document || null,
        phone: form.phone || null,
        email: form.email || null,
        address: form.address || null,
      };
      if (editing) await updateSupplier(editing.id, payload);
      else await createSupplier(payload);
      setShowForm(false);
      load();
    } catch (err) {
      setFormError(err instanceof AdminApiError ? err.message : 'Não foi possível salvar o fornecedor.');
    } finally {
      setBusy(false);
    }
  };

  const remove = async (s: Supplier) => {
    if (!confirm(`Excluir o fornecedor "${s.name}"?`)) return;
    try {
      await deleteSupplier(s.id);
      load();
    } catch (err) {
      alert(err instanceof AdminApiError ? err.message : 'Não foi possível excluir o fornecedor.');
    }
  };

  return (
    <div>
      <header className="adm-page-header">
        <div>
          <h1>Fornecedores</h1>
          <p>Empresas que fornecem os produtos da loja</p>
        </div>
        <button className="adm-btn adm-btn--primary" onClick={openCreate}>
          + Novo fornecedor
        </button>
      </header>

      {error && <p className="adm-form-error">{error}</p>}

      {loading ? (
        <p className="adm-loading">Carregando...</p>
      ) : items.length === 0 ? (
        <p className="adm-empty">Nenhum fornecedor cadastrado ainda.</p>
      ) : (
        <table className="adm-table">
          <thead>
            <tr>
              <th>Nome</th>
              <th>Contato</th>
              <th>Documento</th>
              <th>Produtos</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {items.map((s) => (
              <tr key={s.id}>
                <td>{s.name}</td>
                <td>
                  {s.email ?? '—'}
                  {s.phone ? ` · ${s.phone}` : ''}
                </td>
                <td>{s.document ?? '—'}</td>
                <td>{s.productCount}</td>
                <td className="adm-table__actions">
                  <button className="adm-link" onClick={() => openEdit(s)}>
                    Editar
                  </button>
                  <button className="adm-link adm-link--danger" onClick={() => remove(s)}>
                    Excluir
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {showForm && (
        <AdminModal title={editing ? 'Editar fornecedor' : 'Novo fornecedor'} onClose={() => setShowForm(false)}>
          <form className="adm-form" onSubmit={submit}>
            <label htmlFor="sup-name">Nome</label>
            <input
              id="sup-name"
              value={form.name}
              onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
              required
            />

            <label htmlFor="sup-doc">CNPJ</label>
            <input
              id="sup-doc"
              value={form.document}
              onChange={(e) => setForm((f) => ({ ...f, document: e.target.value }))}
            />

            <label htmlFor="sup-phone">Telefone</label>
            <input
              id="sup-phone"
              value={form.phone}
              onChange={(e) => setForm((f) => ({ ...f, phone: e.target.value }))}
            />

            <label htmlFor="sup-email">E-mail</label>
            <input
              id="sup-email"
              type="email"
              value={form.email}
              onChange={(e) => setForm((f) => ({ ...f, email: e.target.value }))}
            />

            <label htmlFor="sup-address">Endereço</label>
            <input
              id="sup-address"
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
