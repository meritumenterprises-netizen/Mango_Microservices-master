import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Loader } from '../../components/Loader';
import { applyCoupon, emptyCart, getShoppingCart, removeCoupon, removeProductFromCart } from '../../services/cartService';
import type { ShoppingCart } from '../../types';
import { currency, productImage } from '../../utils/format';

export function CartPage() {
  const [cart, setCart] = useState<ShoppingCart | null>(null);
  const [couponCode, setCouponCode] = useState('');
  const [loading, setLoading] = useState(true);

  async function load() {
    setLoading(true);
    try {
      setCart(await getShoppingCart());
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    load();
  }, []);

  async function onApplyCoupon() {
    await applyCoupon(couponCode);
    toast.success('Coupon applied');
    load();
  }

  async function onRemoveCoupon() {
    await removeCoupon();
    toast.success('Coupon removed');
    load();
  }

  async function onRemoveProduct(cartDetailId: number) {
    await removeProductFromCart(cartDetailId);
    toast.success('Product removed from cart');
    load();
  }

  if (loading) {
    return <Loader message="Loading cart..." />;
  }

  const details = cart?.cartDetails ?? [];

  return (
    <section className="surface">
      <div className="page-title">
        <h1>Shopping Cart</h1>
        {details.length > 0 && (
          <button className="btn btn-outline-danger" onClick={() => emptyCart().then(load)}>
            Empty Cart
          </button>
        )}
      </div>
      {details.length === 0 ? (
        <p className="text-muted mb-0">Your cart is empty.</p>
      ) : (
        <>
          {details.map((detail) => (
            <div className="cart-row" key={detail.cartDetailsId}>
              <img src={productImage(detail.product ?? {})} alt={detail.product?.name ?? detail.productId.toString()} />
              <div>
                <h2>{detail.product?.name}</h2>
                <p className="text-muted mb-0">Quantity: {detail.count}</p>
              </div>
              <strong>{currency((detail.product?.price ?? 0) * detail.count)}</strong>
              <button className="btn btn-outline-danger btn-sm" onClick={() => onRemoveProduct(detail.cartDetailsId)}>
                Remove
              </button>
            </div>
          ))}
          <div className="cart-summary">
            <div className="coupon-box">
              <input
                className="form-control"
                placeholder="Coupon code"
                value={couponCode}
                onChange={(e) => setCouponCode(e.target.value)}
              />
              <button className="btn btn-outline-primary" onClick={onApplyCoupon} disabled={!couponCode}>
                Apply
              </button>
              {cart?.cartHeader.couponCode && (
                <button className="btn btn-outline-secondary" onClick={onRemoveCoupon}>
                  Remove Coupon
                </button>
              )}
            </div>
            <div className="summary-total">
              <span>Total</span>
              <strong>{currency(cart?.cartHeader.cartTotal)}</strong>
            </div>
            <Link className="btn btn-success" to="/cart/checkout">
              Checkout
            </Link>
          </div>
        </>
      )}
    </section>
  );
}
