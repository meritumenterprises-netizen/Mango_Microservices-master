import { FormEvent, useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Loader } from '../../components/Loader';
import { getShoppingCart } from '../../services/cartService';
import { createOrder, createStripeSession } from '../../services/orderService';
import type { ShoppingCart } from '../../types';
import { currency } from '../../utils/format';

export function CheckoutPage() {
  const [cart, setCart] = useState<ShoppingCart | null>(null);
  const [loading, setLoading] = useState(true);
  const [redirecting, setRedirecting] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    getShoppingCart()
      .then((nextCart) => {
        setCart(nextCart);
        if (nextCart?.cartHeader) {
          setCart({
            ...nextCart,
            cartHeader: {
              ...nextCart.cartHeader,
              name: nextCart.cartHeader.name ?? '',
              email: nextCart.cartHeader.email ?? '',
              phone: nextCart.cartHeader.phone ?? ''
            }
          });
        }
      })
      .finally(() => setLoading(false));
  }, []);

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    if (!cart) {
      return;
    }

    setRedirecting(true);
    let leavingForStripe = false;
    try {
      const order = await createOrder(cart);
      order.orderTotalWithCurrency = `$${order.orderTotal.toFixed(2)}`;
      const stripe = await createStripeSession({
        approvedUrl: `${window.location.origin}/cart/confirmation/${order.orderHeaderId}`,
        cancelUrl: `${window.location.origin}/cart/checkout`,
        orderHeader: order
      });

      if (stripe.stripeSessionUrl) {
        leavingForStripe = true;
        window.location.assign(stripe.stripeSessionUrl);
        return;
      }

      toast.success('Order placed');
      navigate(`/cart/confirmation/${order.orderHeaderId}`);
    } finally {
      if (!leavingForStripe) {
        setRedirecting(false);
      }
    }
  }

  if (loading || !cart) {
    return <Loader message="Loading checkout..." />;
  }

  if (redirecting) {
    return <Loader message="Redirecting to Stripe..." />;
  }

  return (
    <form className="surface form-page" onSubmit={onSubmit}>
      <h1>Checkout</h1>
      <input
        className="form-control"
        placeholder="Name"
        value={cart.cartHeader.name}
        onChange={(e) => setCart({ ...cart, cartHeader: { ...cart.cartHeader, name: e.target.value } })}
        required
      />
      <input
        className="form-control"
        placeholder="Email"
        type="email"
        value={cart.cartHeader.email}
        onChange={(e) => setCart({ ...cart, cartHeader: { ...cart.cartHeader, email: e.target.value } })}
        required
      />
      <input
        className="form-control"
        placeholder="Phone"
        value={cart.cartHeader.phone}
        onChange={(e) => setCart({ ...cart, cartHeader: { ...cart.cartHeader, phone: e.target.value } })}
        required
      />
      <div className="summary-total">
        <span>Total</span>
        <strong>{currency(cart.cartHeader.cartTotal)}</strong>
      </div>
      <div className="d-flex gap-2">
        <button className="btn btn-success" type="submit">
          Checkout
        </button>
        <Link className="btn btn-outline-secondary" to="/cart">
          Back to Cart
        </Link>
      </div>
    </form>
  );
}
