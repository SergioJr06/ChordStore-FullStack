import { useState, type FormEvent } from 'react';
import { AdminApiError } from '../api/adminClient';
import * as authApi from '../api/auth';
import { useAdminAuth } from '../context/AdminAuthContext';

export default function AdminAccountPage() {
  const { user, setUser } = useAdminAuth();

  const [username, setUsername] = useState(user?.username ?? '');
  const [email, setEmail] = useState(user?.email ?? '');
  const [accountMsg, setAccountMsg] = useState('');
  const [accountError, setAccountError] = useState('');
  const [accountBusy, setAccountBusy] = useState(false);

  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [passwordMsg, setPasswordMsg] = useState('');
  const [passwordError, setPasswordError] = useState('');
  const [passwordBusy, setPasswordBusy] = useState(false);

  const saveAccount = async (e: FormEvent) => {
    e.preventDefault();
    setAccountError('');
    setAccountMsg('');
    setAccountBusy(true);
    try {
      const updated = await authApi.updateAccount(username, email);
      setUser(updated);
      setAccountMsg('Dados atualizados com sucesso.');
    } catch (err) {
      setAccountError(err instanceof AdminApiError ? err.message : 'Não foi possível salvar os dados.');
    } finally {
      setAccountBusy(false);
    }
  };

  const savePassword = async (e: FormEvent) => {
    e.preventDefault();
    setPasswordError('');
    setPasswordMsg('');

    if (newPassword !== confirmPassword) {
      setPasswordError('A confirmação de senha não confere.');
      return;
    }

    setPasswordBusy(true);
    try {
      await authApi.changePassword(currentPassword, newPassword);
      setPasswordMsg('Senha alterada com sucesso.');
      setCurrentPassword('');
      setNewPassword('');
      setConfirmPassword('');
    } catch (err) {
      setPasswordError(err instanceof AdminApiError ? err.message : 'Não foi possível alterar a senha.');
    } finally {
      setPasswordBusy(false);
    }
  };

  return (
    <div>
      <header className="adm-page-header">
        <h1>Minha Conta</h1>
        <p>Dados de acesso do administrador</p>
      </header>

      <section className="adm-section adm-section--narrow">
        <h2>Usuário e e-mail</h2>
        <form className="adm-form" onSubmit={saveAccount}>
          <label htmlFor="acc-username">Usuário</label>
          <input id="acc-username" value={username} onChange={(e) => setUsername(e.target.value)} required />

          <label htmlFor="acc-email">E-mail</label>
          <input
            id="acc-email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />

          {accountMsg && <p className="adm-form-success">{accountMsg}</p>}
          {accountError && <p className="adm-form-error">{accountError}</p>}

          <div className="adm-form__actions adm-form__actions--start">
            <button type="submit" className="adm-btn adm-btn--primary" disabled={accountBusy}>
              {accountBusy ? 'Salvando...' : 'Salvar dados'}
            </button>
          </div>
        </form>
      </section>

      <section className="adm-section adm-section--narrow">
        <h2>Alterar senha</h2>
        <form className="adm-form" onSubmit={savePassword}>
          <label htmlFor="acc-current">Senha atual</label>
          <input
            id="acc-current"
            type="password"
            value={currentPassword}
            onChange={(e) => setCurrentPassword(e.target.value)}
            required
          />

          <label htmlFor="acc-new">Nova senha</label>
          <input
            id="acc-new"
            type="password"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            minLength={6}
            required
          />

          <label htmlFor="acc-confirm">Confirmar nova senha</label>
          <input
            id="acc-confirm"
            type="password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            minLength={6}
            required
          />

          {passwordMsg && <p className="adm-form-success">{passwordMsg}</p>}
          {passwordError && <p className="adm-form-error">{passwordError}</p>}

          <div className="adm-form__actions adm-form__actions--start">
            <button type="submit" className="adm-btn adm-btn--primary" disabled={passwordBusy}>
              {passwordBusy ? 'Salvando...' : 'Alterar senha'}
            </button>
          </div>
        </form>
      </section>
    </div>
  );
}
