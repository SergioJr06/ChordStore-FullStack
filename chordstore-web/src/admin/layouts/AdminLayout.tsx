import { NavLink, Outlet } from 'react-router-dom';
import { useAdminAuth } from '../context/AdminAuthContext';

const links = [
  { to: '/admin', label: 'Dashboard', end: true },
  { to: '/admin/produtos', label: 'Produtos' },
  { to: '/admin/categorias', label: 'Categorias' },
  { to: '/admin/fornecedores', label: 'Fornecedores' },
  { to: '/admin/clientes', label: 'Clientes' },
  { to: '/admin/vendas', label: 'Vendas' },
  { to: '/admin/conta', label: 'Minha Conta' },
];

export default function AdminLayout() {
  const { user, logout } = useAdminAuth();

  return (
    <div className="adm-shell">
      <aside className="adm-sidebar">
        <div className="adm-sidebar__brand">
          <img src="/images/logo.png" alt="ChordStore" />
          <span>Admin</span>
        </div>

        <nav className="adm-sidebar__nav">
          {links.map((link) => (
            <NavLink
              key={link.to}
              to={link.to}
              end={link.end}
              className={({ isActive }) => `adm-sidebar__link${isActive ? ' adm-sidebar__link--active' : ''}`}
            >
              {link.label}
            </NavLink>
          ))}
        </nav>

        <div className="adm-sidebar__footer">
          <p className="adm-sidebar__user">{user?.username}</p>
          <button type="button" className="adm-btn adm-btn--ghost" onClick={logout}>
            Sair
          </button>
        </div>
      </aside>

      <div className="adm-content">
        <Outlet />
      </div>
    </div>
  );
}
