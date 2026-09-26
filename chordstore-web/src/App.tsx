import { Route, Routes } from 'react-router-dom';
import ProtectedRoute from './admin/components/ProtectedRoute';
import { AdminAuthProvider } from './admin/context/AdminAuthContext';
import AdminLayout from './admin/layouts/AdminLayout';
import AdminAccountPage from './admin/pages/AdminAccountPage';
import AdminCategoriesPage from './admin/pages/AdminCategoriesPage';
import AdminCustomersPage from './admin/pages/AdminCustomersPage';
import AdminDashboardPage from './admin/pages/AdminDashboardPage';
import AdminLoginPage from './admin/pages/AdminLoginPage';
import AdminProductsPage from './admin/pages/AdminProductsPage';
import AdminSalesPage from './admin/pages/AdminSalesPage';
import AdminSuppliersPage from './admin/pages/AdminSuppliersPage';
import SiteLayout from './layouts/SiteLayout';
import HomePage from './pages/HomePage';
import PlanosPage from './pages/PlanosPage';

export default function App() {
  return (
    <AdminAuthProvider>
      <Routes>
        {/* Site público — igual estava antes, agora com mais uma página (Planos) */}
        <Route element={<SiteLayout />}>
          <Route path="/" element={<HomePage />} />
          <Route path="/planos" element={<PlanosPage />} />
        </Route>

        {/* Login do admin (fora do layout protegido) */}
        <Route path="/admin/login" element={<AdminLoginPage />} />

        {/* Área administrativa protegida */}
        <Route
          path="/admin"
          element={
            <ProtectedRoute>
              <AdminLayout />
            </ProtectedRoute>
          }
        >
          <Route index element={<AdminDashboardPage />} />
          <Route path="produtos" element={<AdminProductsPage />} />
          <Route path="categorias" element={<AdminCategoriesPage />} />
          <Route path="fornecedores" element={<AdminSuppliersPage />} />
          <Route path="clientes" element={<AdminCustomersPage />} />
          <Route path="vendas" element={<AdminSalesPage />} />
          <Route path="conta" element={<AdminAccountPage />} />
        </Route>
      </Routes>
    </AdminAuthProvider>
  );
}
