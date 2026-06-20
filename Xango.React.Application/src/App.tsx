import { Navigate, Route, Routes } from 'react-router-dom';
import { Layout } from './layout/Layout';
import { HomePage } from './pages/HomePage';
import { LoginPage } from './pages/auth/LoginPage';
import { LogoutPage } from './pages/auth/LogoutPage';
import { RegisterPage } from './pages/auth/RegisterPage';
import { CartPage } from './pages/cart/CartPage';
import { CheckoutPage } from './pages/cart/CheckoutPage';
import { ConfirmationPage } from './pages/cart/ConfirmationPage';
import { PlaceOrderPage } from './pages/cart/PlaceOrderPage';
import { CouponFormPage } from './pages/coupons/CouponFormPage';
import { CouponListPage } from './pages/coupons/CouponListPage';
import { OrderDetailsPage } from './pages/orders/OrderDetailsPage';
import { OrdersPage } from './pages/orders/OrdersPage';
import { ProductDetailsPage } from './pages/products/ProductDetailsPage';
import { ProductFormPage } from './pages/products/ProductFormPage';
import { ProductListPage } from './pages/products/ProductListPage';
import { RequireLogin } from './components/RequireLogin';

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<HomePage />} />
        <Route path="login" element={<LoginPage />} />
        <Route path="register" element={<RegisterPage />} />
        <Route path="logout" element={<LogoutPage />} />
        <Route path="product/details/:id" element={<ProductDetailsPage />} />
        <Route
          path="coupon"
          element={
            <RequireLogin>
              <CouponListPage />
            </RequireLogin>
          }
        />
        <Route
          path="coupon/create"
          element={
            <RequireLogin>
              <CouponFormPage />
            </RequireLogin>
          }
        />
        <Route
          path="coupon/edit/:id"
          element={
            <RequireLogin>
              <CouponFormPage />
            </RequireLogin>
          }
        />
        <Route
          path="coupon/delete/:id"
          element={
            <RequireLogin>
              <CouponListPage />
            </RequireLogin>
          }
        />
        <Route
          path="product"
          element={
            <RequireLogin>
              <ProductListPage />
            </RequireLogin>
          }
        />
        <Route
          path="product/create"
          element={
            <RequireLogin>
              <ProductFormPage />
            </RequireLogin>
          }
        />
        <Route
          path="product/edit/:id"
          element={
            <RequireLogin>
              <ProductFormPage />
            </RequireLogin>
          }
        />
        <Route
          path="product/delete/:id"
          element={
            <RequireLogin>
              <ProductListPage />
            </RequireLogin>
          }
        />
        <Route
          path="order"
          element={
            <RequireLogin>
              <OrdersPage />
            </RequireLogin>
          }
        />
        <Route
          path="order/:status"
          element={
            <RequireLogin>
              <OrdersPage />
            </RequireLogin>
          }
        />
        <Route
          path="order/details/:id"
          element={
            <RequireLogin>
              <OrderDetailsPage />
            </RequireLogin>
          }
        />
        <Route
          path="cart"
          element={
            <RequireLogin>
              <CartPage />
            </RequireLogin>
          }
        />
        <Route
          path="cart/checkout"
          element={
            <RequireLogin>
              <CheckoutPage />
            </RequireLogin>
          }
        />
        <Route
          path="cart/placeorder"
          element={
            <RequireLogin>
              <PlaceOrderPage />
            </RequireLogin>
          }
        />
        <Route path="cart/confirmation/:id" element={<ConfirmationPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Route>
    </Routes>
  );
}
