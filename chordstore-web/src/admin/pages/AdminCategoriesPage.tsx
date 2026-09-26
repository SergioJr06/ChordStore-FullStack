import { useEffect, useState, type FormEvent } from 'react';
import AdminModal from '../components/AdminModal';
import { AdminApiError } from '../api/adminClient';
import { createCategory, deleteCategory, listCategories, updateCategory } from '../api/categories';
import type { Category } from '../types';

const emptyForm = { name: '', description: '' };

export default function AdminCategoriesPage() {
  const [items, setItems] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [editing, setEditing] = useState<Category | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState(emptyForm);
  const [formError, setFormError] = useState('');
  const [busy, setBusy] = useState(false);

  const load = () => {
    setLoading(true);
    listCategories()
      .then(setItems)
      .catch(() => setError('Não foi possível carregar as categorias.'))
      .finally(() => setLoading(false));
  };

  useEffect(load, []);

  const openCreate = () => {
    setEditing(null);
    setForm(emptyForm);
    setFormError('');
    setShowForm(true);
  };

  const openEdit = (c: Category) => {
    setEditing(c);
    setForm({ name: c.name, description: c.description ?? '' });
    setFormError('');
    setShowForm(true);
  };

  const submit = async (e: FormEvent) => {
    e.preventDefault();
    setFormError('');
    setBusy(true);
    try {
      const payload = { name: form.name, description: form.description || null };
      if (editing) await updateCategory(editing.id, payload);
      else await createCategory(payload);
      setShowForm(false);
      load();
    } catch (err) {
      setFormError(err instanceof AdminApiError ? err.message : 'Não foi possível salvar a categoria.');
    } finally {
      setBusy(false);
    }
  };

  const remove = async (c: Category) => {
    if (!confirm(`Excluir a categoria "${c.name}"?`)) return;
    try {
      await deleteCategory(c.id);
      load();
    } catch (err) {
      alert(err instanceof AdminApiError ? err.message : 'Não foi possível excluir a categoria.');
    }
  };

  return (
    <div>
      <header className="adm-page-header">
        <div>
          <h1>Categorias</h1>
          <p>Organize os produtos por categoria</p>
        </div>
        <button className="adm-btn adm-btn--primary" onClick={openCreate}>
          + Nova categoria
        </button>
      </header>

      {error && <p className="adm-form-error">{error}</p>}

      {loading ? (
        <p className="adm-loading">Carregando...</p>
      ) : items.length === 0 ? (
        <p className="adm-empty">Nenhuma categoria cadastrada ainda.</p>
      ) : (
        <table className="adm-table">
          <thead>
            <tr>
              <th>Nome</th>
              <th>Descrição</th>
              <th>Produtos</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {items.map((c) => (
              <tr key={c.id}>
                <td>{c.name}</td>
                <td>{c.description ?? '—'}</td>
                <td>{c.productCount}</td>
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
        <AdminModal title={editing ? 'Editar categoria' : 'Nova categoria'} onClose={() => setShowForm(false)}>
          <form className="adm-form" onSubmit={submit}>
            <label htmlFor="cat-name">Nome</label>
            <input
              id="cat-name"
              value={form.name}
              onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
              required
            />

            <label htmlFor="cat-desc">Descrição</label>
            <textarea
              id="cat-desc"
              rows={3}
              value={form.description}
              onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
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
