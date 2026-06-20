import { NavLink } from 'react-router-dom';
import { useAuth } from '../state/AuthContext';

export function Header() {
  const { isLoggedIn, user } = useAuth();

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
      <div className="container">
        <NavLink className="navbar-brand fw-semibold" to="/">
          Xango
        </NavLink>
        <button
          className="navbar-toggler"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#mainNavigation"
          aria-controls="mainNavigation"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon" />
        </button>
        <div className="collapse navbar-collapse" id="mainNavigation">
          <ul className="navbar-nav me-auto mb-2 mb-lg-0">
            <li className="nav-item">
              <NavLink className="nav-link" to="/">
                Home
              </NavLink>
            </li>
            {isLoggedIn && (
              <>
                <li className="nav-item dropdown">
                  <button className="nav-link dropdown-toggle btn btn-link" data-bs-toggle="dropdown">
                    Management
                  </button>
                  <ul className="dropdown-menu">
                    <li>
                      <NavLink className="dropdown-item" to="/coupon">
                        Coupon
                      </NavLink>
                    </li>
                    <li>
                      <NavLink className="dropdown-item" to="/product">
                        Product
                      </NavLink>
                    </li>
                  </ul>
                </li>
                <li className="nav-item">
                  <NavLink className="nav-link" to="/order">
                    Orders
                  </NavLink>
                </li>
                <li className="nav-item">
                  <NavLink className="nav-link" to="/cart">
                    Cart
                  </NavLink>
                </li>
              </>
            )}
          </ul>
          <ul className="navbar-nav ms-auto">
            {isLoggedIn ? (
              <>
                <li className="nav-item">
                  <span className="nav-link text-light">{user?.name || user?.email}</span>
                </li>
                <li className="nav-item">
                  <NavLink className="nav-link" to="/logout">
                    Logout
                  </NavLink>
                </li>
              </>
            ) : (
              <>
                <li className="nav-item">
                  <NavLink className="nav-link" to="/login">
                    Login
                  </NavLink>
                </li>
                <li className="nav-item">
                  <NavLink className="nav-link" to="/register">
                    Register
                  </NavLink>
                </li>
              </>
            )}
          </ul>
        </div>
      </div>
    </nav>
  );
}
