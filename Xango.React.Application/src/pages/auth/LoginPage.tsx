import { FormEvent, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { login } from '../../services/authService';
import { useAuth } from '../../state/AuthContext';

export function LoginPage() {
  const [userName, setUserName] = useState('');
  const [password, setPassword] = useState('');
  const [busy, setBusy] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const { loginComplete } = useAuth();

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    try {
      const result = await login(userName, password);
      loginComplete(result.token, result.user);
      toast.success('Login successful');
      navigate((location.state as { from?: string } | null)?.from ?? '/');
    } finally {
      setBusy(false);
    }
  }

  return (
    <form className="auth-form" onSubmit={onSubmit}>
      <h1>Login</h1>
      <label className="form-label">
        Email
        <input className="form-control" value={userName} onChange={(e) => setUserName(e.target.value)} required />
      </label>
      <label className="form-label">
        Password
        <input
          className="form-control"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
      </label>
      <button className="btn btn-primary w-100" type="submit" disabled={busy}>
        {busy ? 'Signing in...' : 'Login'}
      </button>
    </form>
  );
}
