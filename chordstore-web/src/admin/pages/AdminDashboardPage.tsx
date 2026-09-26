import { useEffect, useState } from 'react';
import { listCategories } from '../api/categories';
import { listCustomers } from '../api/customers';
import { listProducts } from '../api/products';
import { listSales } from '../api/sales';
import { listSuppliers } from '../api/suppliers';
import type { AdminProduct, Category, Customer, Sale, Supplier } from '../types';
import { money } from '../../utils/format';

export default function AdminDashboardPage() {
  const [products, setProducts] = useState<AdminProduct[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [sales, setSales] = useState<Sale[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    Promise.all([listProducts(), listCategories(), listSuppliers(), listCustomers(), listSales()])
      .then(([p, c, s, cu, sa]) => {
        setProducts(p);
        setCategories(c);
        setSuppliers(s);
        setCustomers(cu);
        setSales(sa);
      })
      .catch(() => setError('Não foi possível carregar os dados do painel.'))
      .finally(() => setLoading(false));
  }, []);

  const now = new Date();
  const salesThisMonth = sales.filter((s) => {
    const d = new Date(s.createdAt);
    return d.getMonth() === now.getMonth() && d.getFullYear() === now.getFullYear() && s.status !== 'Cancelado';
  });
  const revenueThisMonth = salesThisMonth.reduce((sum, s) => sum + s.total, 0);
  const lowStock = products.filter((p) => p.stockQuantity <= 3);

  if (loading) return <p className="adm-loading">Carregando painel...</p>;

  return (
    <div>
      <header className="adm-page-header">
        <h1>Dashboard</h1>
        <p>Resumo geral da loja</p>
      </header>

      {error && <p className="adm-form-error">{error}</p>}

      <div className="adm-cards">
        <div className="adm-card">
          <span className="adm-card__label">Produtos cadastrados</span>
          <strong className="adm-card__value">{products.length}</strong>
        </div>
        <div className="adm-card">
          <span className="adm-card__label">Categorias</span>
          <strong className="adm-card__value">{categories.length}</strong>
        </div>
        <div className="adm-card">
          <span className="adm-card__label">Fornecedores</span>
          <strong className="adm-card__value">{suppliers.length}</strong>
        </div>
        <div className="adm-card">
          <span className="adm-card__label">Clientes</span>
          <strong className="adm-card__value">{customers.length}</strong>
        </div>
        <div className="adm-card">
          <span className="adm-card__label">Vendas no mês</span>
          <strong className="adm-card__value">{salesThisMonth.length}</strong>
        </div>
        <div className="adm-card adm-card--highlight">
          <span className="adm-card__label">Faturamento no mês</span>
          <strong className="adm-card__value">{money(revenueThisMonth)}</strong>
        </div>
      </div>

      <section className="adm-section">
        <h2>Estoque baixo</h2>
        {lowStock.length === 0 ? (
          <p className="adm-empty">Nenhum produto com estoque crítico. 🎉</p>
        ) : (
          <table className="adm-table">
            <thead>
              <tr>
                <th>Produto</th>
                <th>Categoria</th>
                <th>Estoque</th>
              </tr>
            </thead>
            <tbody>
              {lowStock.map((p) => (
                <tr key={p.id}>
                  <td>{p.name}</td>
                  <td>{p.categoryName ?? '—'}</td>
                  <td className="adm-table__danger">{p.stockQuantity}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>

      <section className="adm-section">
        <h2>Últimas vendas</h2>
        {sales.length === 0 ? (
          <p className="adm-empty">Ainda não há vendas registradas.</p>
        ) : (
          <table className="adm-table">
            <thead>
              <tr>
                <th>#</th>
                <th>Cliente</th>
                <th>Data</th>
                <th>Status</th>
                <th>Total</th>
              </tr>
            </thead>
            <tbody>
              {sales.slice(0, 5).map((s) => (
                <tr key={s.id}>
                  <td>#{s.id}</td>
                  <td>{s.customerName}</td>
                  <td>{new Date(s.createdAt).toLocaleDateString('pt-BR')}</td>
                  <td>
                    <span className={`adm-badge adm-badge--${s.status.toLowerCase()}`}>{s.status}</span>
                  </td>
                  <td>{money(s.total)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>
    </div>
  );
}
