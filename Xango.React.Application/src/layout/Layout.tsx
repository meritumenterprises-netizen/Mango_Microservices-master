import { Outlet } from 'react-router-dom';
import { Header } from './Header';
import { Footer } from './Footer';

export function Layout() {
  return (
    <div className="app-shell">
      <Header />
      <main className="container py-4">
        <Outlet />
      </main>
      <Footer />
    </div>
  );
}
