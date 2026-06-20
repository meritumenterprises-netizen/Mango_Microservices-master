import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Loader } from '../../components/Loader';
import { getShoppingCart } from '../../services/cartService';
import { createOrder, createStripeSession } from '../../services/orderService';
import type { ShoppingCart } from '../../types';
import { currency } from '../../utils/format';

let stripeRedirectInProgress = false;

export function PlaceOrderPage() {
  const navigate = useNavigate();
  const [busy, setBusy] = useState(false);
  const [loading, setLoading] = useState(true);
  const [cart, setCart] = useState<ShoppingCart | null>(null);

  useEffect(() => {
    const rawUserDetails = sessionStorage.getItem('userDetails');
    const userDetails = rawUserDetails
      ? (JSON.parse(rawUserDetails) as { name: string; email: string; phone: string })
      : null;
    sessionStorage.removeItem('userDetails');

    getShoppingCart()
      .then((shoppingCart) => {
        if (!shoppingCart || !userDetails) {
          navigate('/cart');
          return;
        }
        setCart({
          ...shoppingCart,
          cartHeader: {
            ...shoppingCart.cartHeader,
            name: userDetails.name,
            email: userDetails.email,
            phone: userDetails.phone
          }
        });
      })
      .finally(() => setLoading(false));
  }, [navigate]);

  useEffect(() => {
    if (!cart || stripeRedirectInProgress) {
      return;
    }

    async function placeOrderAndRedirect() {
      if (!cart) {
        navigate('/cart');
        return;
      }

      stripeRedirectInProgress = true;
      setBusy(true);
      try {
        const order = await createOrder(cart);
        order.orderTotalWithCurrency = `$${order.orderTotal.toFixed(2)}`;
        const stripe = await createStripeSession({
          approvedUrl: `${window.location.origin}/cart/confirmation/${order.orderHeaderId}`,
          cancelUrl: `${window.location.origin}/cart/checkout`,
          orderHeader: order
        });

        if (stripe.stripeSessionUrl) {
          window.location.href = stripe.stripeSessionUrl;
          return;
        }

        toast.success('Order placed');
        stripeRedirectInProgress = false;
        navigate(`/cart/confirmation/${order.orderHeaderId}`);
      } catch {
        stripeRedirectInProgress = false;
      } finally {
        setBusy(false);
      }
    }

    placeOrderAndRedirect();
  }, [cart, navigate]);

  function onBackToCart() {
    setBusy(true);
    navigate('/cart');
  }

  if (loading) {
    return <Loader message="Preparing order..." />;
  }

  if (!cart) {
    return (
      <section className="surface">
        <h1>Place Order</h1>
        <p className="text-muted">No checkout information was found.</p>
      </section>
    );
  }

  return (
    <section className="surface form-page">
      <h1>Redirecting to Stripe</h1>
      <p>{cart.cartHeader.name}</p>
      <p>{cart.cartHeader.email}</p>
      <p>{cart.cartHeader.phone}</p>
      <div className="summary-total">
        <span>Total</span>
        <strong>{currency(cart.cartHeader.cartTotal)}</strong>
      </div>
      <button className="btn btn-outline-secondary" onClick={onBackToCart} disabled={busy}>
        Back to Cart
      </button>
    </section>
  );
}
