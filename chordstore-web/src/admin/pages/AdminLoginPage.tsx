import { useState, type FormEvent } from 'react';
import { Navigate, useLocation, useNavigate } from 'react-router-dom';
import { AdminApiError } from '../api/adminClient';
import { useAdminAuth } from '../context/AdminAuthContext';

export default function AdminLoginPage() {
  const { login, isAuthenticated, loading } = useAdminAuth();
  const navigate = useNavigate();
  const location = useLocation() as { state?: { from?: { pathname: string } } };

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);

  if (!loading && isAuthenticated) {
    return <Navigate to={location.state?.from?.pathname ?? '/admin'} replace />;
  }

  const submit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setBusy(true);
    try {
      await login(email, password);
      navigate(location.state?.from?.pathname ?? '/admin', { replace: true });
    } catch (err) {
      setError(err instanceof AdminApiError ? err.message : 'Não foi possível entrar. Tente novamente.');
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="adm-login">
      <form className="adm-login__card" onSubmit={submit}>
        <img src="/images/logo.png" alt="ChordStore" className="adm-login__logo" />
        <h1>Painel Administrativo</h1>
        <p className="adm-login__subtitle">Entre com seu usuário e senha de admin</p>

        <label htmlFor="adm-email">E-mail ou usuário</label>
        <input
          id="adm-email"
          type="text"
          autoComplete="username"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />

        <label htmlFor="adm-password">Senha</label>
        <input
          id="adm-password"
          type="password"
          autoComplete="current-password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />

        {error && (
          <p className="adm-form-error" role="alert">
            {error}
          </p>
        )}

        <button type="submit" className="adm-btn adm-btn--primary" disabled={busy}>
          {busy ? 'Entrando...' : 'Entrar'}
        </button>

        <p className="adm-login__hint">Padrão de fábrica: contato@chordstore.com / admin123</p>
      </form>
    </div>
  );
}
