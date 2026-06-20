import { createContext, useContext, useMemo, useState } from 'react';
import type { ReactNode } from 'react';
import type { UserRecord } from '../types';
import { clearAuth, getToken, getUser, setToken, setUser } from '../services/authStorage';

interface AuthContextValue {
  user: UserRecord | null;
  token: string | null;
  isLoggedIn: boolean;
  loginComplete: (token: string, user: UserRecord) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setCurrentUser] = useState<UserRecord | null>(() => getUser());
  const [token, setCurrentToken] = useState<string | null>(() => getToken());

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      token,
      isLoggedIn: Boolean(user && token),
      loginComplete(nextToken, nextUser) {
        setToken(nextToken);
        setUser(nextUser);
        setCurrentToken(nextToken);
        setCurrentUser(nextUser);
      },
      logout() {
        clearAuth();
        setCurrentToken(null);
        setCurrentUser(null);
      }
    }),
    [user, token]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used inside AuthProvider');
  }
  return context;
}
