import { useEffect, useState, type FormEvent } from 'react';
import AdminModal from '../components/AdminModal';
import { AdminApiError } from '../api/adminClient';
import { listCategories } from '../api/categories';
import { createProduct, deleteProduct, listProducts, updateProduct } from '../api/products';
import { listSuppliers } from '../api/suppliers';
import type { AdminProduct, Category, Supplier } from '../types';
import { money } from '../../utils/format';

interface FormState {
  slug: string;
  name: string;
  section: string;
  price: string;
  oldPrice: string;
  installments: string;
  description: string;
  imageUrl: string;
  galleryUrls: string;
  brand: string;
  stockQuantity: string;
  categoryId: string;
  supplierId: string;
}

const emptyForm: FormState = {
  slug: '',
  name: '',
  section: 'novo',
  price: '',
  oldPrice: '',
  installments: '12',
  description: '',
  imageUrl: '',
  galleryUrls: '',
  brand: '',
  stockQuantity: '0',
  categoryId: '',
  supplierId: '',
};

export default function AdminProductsPage() {
  const [items, setItems] = useState<AdminProduct[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [search, setSearch] = useState('');

  const [editing, setEditing] = useState<AdminProduct | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [formError, setFormError] = useState('');
  const [busy, setBusy] = useState(false);

  const load = (term?: string) => {
    setLoading(true);
    listProducts(term)
      .then(setItems)
      .catch(() => setError('Não foi possível carregar os produtos.'))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    load();
    listCategories().then(setCategories).catch(() => {});
    listSuppliers().then(setSuppliers).catch(() => {});
  }, []);

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

  const openEdit = (p: AdminProduct) => {
    setEditing(p);
    setForm({
      slug: p.slug,
      name: p.name,
      section: p.section,
      price: p.price != null ? String(p.price) : '',
      oldPrice: p.oldPrice != null ? String(p.oldPrice) : '',
      installments: String(p.installments),
      description: p.description ?? '',
      imageUrl: p.imageUrl,
      galleryUrls: p.galleryUrls.join(', '),
      brand: p.brand,
      stockQuantity: String(p.stockQuantity),
      categoryId: p.categoryId != null ? String(p.categoryId) : '',
      supplierId: p.supplierId != null ? String(p.supplierId) : '',
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
        slug: form.slug.trim(),
        name: form.name.trim(),
        section: form.section,
        price: form.price === '' ? null : Number(form.price),
        oldPrice: form.oldPrice === '' ? null : Number(form.oldPrice),
        installments: Number(form.installments) || 1,
        description: form.description || null,
        imageUrl: form.imageUrl.trim(),
        galleryUrls: form.galleryUrls
          .split(',')
          .map((g) => g.trim())
          .filter(Boolean),
        brand: form.brand.trim(),
        stockQuantity: Number(form.stockQuantity) || 0,
        categoryId: form.categoryId === '' ? null : Number(form.categoryId),
        supplierId: form.supplierId === '' ? null : Number(form.supplierId),
      };
      if (editing) await updateProduct(editing.id, payload);
      else await createProduct(payload);
      setShowForm(false);
      load(search);
    } catch (err) {
      setFormError(err instanceof AdminApiError ? err.message : 'Não foi possível salvar o produto.');
    } finally {
      setBusy(false);
    }
  };

  const remove = async (p: AdminProduct) => {
    if (!confirm(`Excluir o produto "${p.name}"?`)) return;
    try {
      await deleteProduct(p.id);
      load(search);
    } catch (err) {
      alert(err instanceof AdminApiError ? err.message : 'Não foi possível excluir o produto.');
    }
  };

  return (
    <div>
      <header className="adm-page-header">
        <div>
          <h1>Produtos</h1>
          <p>Cadastro de instrumentos da loja</p>
        </div>
        <button className="adm-btn adm-btn--primary" onClick={openCreate}>
          + Novo produto
        </button>
      </header>

      <form className="adm-search" onSubmit={runSearch}>
        <input placeholder="Buscar por nome ou slug..." value={search} onChange={(e) => setSearch(e.target.value)} />
        <button type="submit" className="adm-btn adm-btn--ghost">
          Buscar
        </button>
      </form>

      {error && <p className="adm-form-error">{error}</p>}

      {loading ? (
        <p className="adm-loading">Carregando...</p>
      ) : items.length === 0 ? (
        <p className="adm-empty">Nenhum produto encontrado.</p>
      ) : (
        <table className="adm-table">
          <thead>
            <tr>
              <th>Produto</th>
              <th>Categoria</th>
              <th>Fornecedor</th>
              <th>Preço</th>
              <th>Estoque</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {items.map((p) => (
              <tr key={p.id}>
                <td>
                  <div className="adm-table__product">
                    <img src={p.imageUrl} alt="" onError={(e) => (e.currentTarget.style.visibility = 'hidden')} />
                    <div>
                      <strong>{p.name}</strong>
                      <span>{p.brand}</span>
                    </div>
                  </div>
                </td>
                <td>{p.categoryName ?? '—'}</td>
                <td>{p.supplierName ?? '—'}</td>
                <td>{p.price != null ? money(p.price) : 'Sob consulta'}</td>
                <td className={p.stockQuantity <= 3 ? 'adm-table__danger' : ''}>{p.stockQuantity}</td>
                <td className="adm-table__actions">
                  <button className="adm-link" onClick={() => openEdit(p)}>
                    Editar
                  </button>
                  <button className="adm-link adm-link--danger" onClick={() => remove(p)}>
                    Excluir
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {showForm && (
        <AdminModal title={editing ? 'Editar produto' : 'Novo produto'} onClose={() => setShowForm(false)} width={640}>
          <form className="adm-form" onSubmit={submit}>
            <div className="adm-form__grid">
              <div>
                <label htmlFor="p-name">Nome</label>
                <input
                  id="p-name"
                  value={form.name}
                  onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
                  required
                />
              </div>
              <div>
                <label htmlFor="p-slug">Slug (URL)</label>
                <input
                  id="p-slug"
                  value={form.slug}
                  onChange={(e) => setForm((f) => ({ ...f, slug: e.target.value }))}
                  required
                />
              </div>
            </div>

            <div className="adm-form__grid">
              <div>
                <label htmlFor="p-section">Seção</label>
                <select
                  id="p-section"
                  value={form.section}
                  onChange={(e) => setForm((f) => ({ ...f, section: e.target.value }))}
                >
                  <option value="novo">Novo</option>
                  <option value="exclusivo">Exclusivo</option>
                  <option value="promocao">Promoção</option>
                </select>
              </div>
              <div>
                <label htmlFor="p-brand">Marca</label>
                <input
                  id="p-brand"
                  value={form.brand}
                  onChange={(e) => setForm((f) => ({ ...f, brand: e.target.value }))}
                />
              </div>
            </div>

            <div className="adm-form__grid">
              <div>
                <label htmlFor="p-category">Categoria</label>
                <select
                  id="p-category"
                  value={form.categoryId}
                  onChange={(e) => setForm((f) => ({ ...f, categoryId: e.target.value }))}
                >
                  <option value="">Sem categoria</option>
                  {categories.map((c) => (
                    <option key={c.id} value={c.id}>
                      {c.name}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <label htmlFor="p-supplier">Fornecedor</label>
                <select
                  id="p-supplier"
                  value={form.supplierId}
                  onChange={(e) => setForm((f) => ({ ...f, supplierId: e.target.value }))}
                >
                  <option value="">Sem fornecedor</option>
                  {suppliers.map((s) => (
                    <option key={s.id} value={s.id}>
                      {s.name}
                    </option>
                  ))}
                </select>
              </div>
            </div>

            <div className="adm-form__grid">
              <div>
                <label htmlFor="p-price">Preço (R$) — vazio = sob consulta</label>
                <input
                  id="p-price"
                  type="number"
                  step="0.01"
                  value={form.price}
                  onChange={(e) => setForm((f) => ({ ...f, price: e.target.value }))}
                />
              </div>
              <div>
                <label htmlFor="p-oldprice">Preço antigo (R$)</label>
                <input
                  id="p-oldprice"
                  type="number"
                  step="0.01"
                  value={form.oldPrice}
                  onChange={(e) => setForm((f) => ({ ...f, oldPrice: e.target.value }))}
                />
              </div>
            </div>

            <div className="adm-form__grid">
              <div>
                <label htmlFor="p-installments">Parcelas máx.</label>
                <input
                  id="p-installments"
                  type="number"
                  min={1}
                  value={form.installments}
                  onChange={(e) => setForm((f) => ({ ...f, installments: e.target.value }))}
                />
              </div>
              <div>
                <label htmlFor="p-stock">Estoque</label>
                <input
                  id="p-stock"
                  type="number"
                  min={0}
                  value={form.stockQuantity}
                  onChange={(e) => setForm((f) => ({ ...f, stockQuantity: e.target.value }))}
                />
              </div>
            </div>

            <label htmlFor="p-image">URL da imagem principal</label>
            <input
              id="p-image"
              value={form.imageUrl}
              onChange={(e) => setForm((f) => ({ ...f, imageUrl: e.target.value }))}
              required
            />

            <label htmlFor="p-gallery">Galeria (URLs separadas por vírgula)</label>
            <input
              id="p-gallery"
              value={form.galleryUrls}
              onChange={(e) => setForm((f) => ({ ...f, galleryUrls: e.target.value }))}
            />

            <label htmlFor="p-desc">Descrição</label>
            <textarea
              id="p-desc"
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
