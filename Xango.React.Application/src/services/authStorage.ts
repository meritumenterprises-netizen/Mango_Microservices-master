import type { UserRecord } from '../types';

const tokenKey = 'auth_token';
const userKey = 'user_value';

export function setToken(token: string) {
  localStorage.setItem(tokenKey, token);
}

export function getToken() {
  return localStorage.getItem(tokenKey);
}

export function setUser(user: UserRecord) {
  localStorage.setItem(userKey, JSON.stringify(user));
}

export function getUser(): UserRecord | null {
  const value = localStorage.getItem(userKey);
  return value ? (JSON.parse(value) as UserRecord) : null;
}

export function clearAuth() {
  localStorage.removeItem(tokenKey);
  localStorage.removeItem(userKey);
}
