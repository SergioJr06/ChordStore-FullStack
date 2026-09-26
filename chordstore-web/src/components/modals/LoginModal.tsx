import { useState, type FormEvent } from 'react';
import { login } from '../../api/auth';
import { useUI } from '../../context/UIContext';
import Modal from './Modal';

export default function LoginModal() {
  const { close } = useUI();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);

  const submit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setBusy(true);
    try {
      const { token } = await login(email, password);
      localStorage.setItem('chordstore:token', token);
      close();
    } catch {
      setError('Não foi possível entrar. Confira o e-mail e a senha e tente de novo.');
    } finally {
      setBusy(false);
    }
  };

  return (
    <Modal label="Entrar" onClose={close} exitIcon="/images/exit-usuario.svg" exitWidth={18} className="modal--login">
      <form onSubmit={submit}>
        <h2 className="pill-title">FAÇA LOGIN COM SEU ENDEREÇO DE E-MAIL</h2>

        <label htmlFor="login-email">Endereço de e-mail</label>
        <input
          id="login-email"
          type="email"
          autoComplete="email"
          placeholder="e-mail"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />

        <label htmlFor="login-pass">Senha</label>
        <input
          id="login-pass"
          type="password"
          autoComplete="current-password"
          placeholder="senha"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />

        {error && (
          <p className="form-error" role="alert">
            {error}
          </p>
        )}
        <button className="login__submit" type="submit" disabled={busy}>
          ENTRAR
        </button>
      </form>
    </Modal>
  );
}
