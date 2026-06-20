import { FormEvent, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { register } from '../../services/authService';
import type { RegistrationRequest } from '../../types';

const initialForm: RegistrationRequest = {
  email: '',
  name: '',
  phoneNumber: '',
  password: '',
  role: 'Customer'
};

export function RegisterPage() {
  const [form, setForm] = useState(initialForm);
  const [busy, setBusy] = useState(false);
  const navigate = useNavigate();

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    try {
      await register(form);
      toast.success('User registration successful');
      navigate('/');
    } finally {
      setBusy(false);
    }
  }

  return (
    <form className="auth-form" onSubmit={onSubmit}>
      <h1>Register</h1>
      <input
        className="form-control"
        placeholder="Name"
        value={form.name}
        onChange={(e) => setForm({ ...form, name: e.target.value })}
        required
      />
      <input
        className="form-control"
        placeholder="Email"
        type="email"
        value={form.email}
        onChange={(e) => setForm({ ...form, email: e.target.value })}
        required
      />
      <input
        className="form-control"
        placeholder="Phone number"
        value={form.phoneNumber}
        onChange={(e) => setForm({ ...form, phoneNumber: e.target.value })}
        required
      />
      <input
        className="form-control"
        placeholder="Password"
        type="password"
        value={form.password}
        onChange={(e) => setForm({ ...form, password: e.target.value })}
        required
      />
      <select className="form-select" value={form.role} onChange={(e) => setForm({ ...form, role: e.target.value })}>
        <option value="Customer">Customer</option>
        <option value="ADMIN">Admin</option>
      </select>
      <button className="btn btn-primary w-100" type="submit" disabled={busy}>
        {busy ? 'Creating account...' : 'Register'}
      </button>
    </form>
  );
}
