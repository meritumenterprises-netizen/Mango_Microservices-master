import { useEffect, useRef, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Loader } from '../../components/Loader';
import { deleteCart } from '../../services/cartService';
import { validateStripeSession } from '../../services/orderService';
import { useAuth } from '../../state/AuthContext';

export function ConfirmationPage() {
  const { id } = useParams();
  const { user } = useAuth();
  const [loading, setLoading] = useState(Boolean(id));
  const confirmationStarted = useRef(false);

  useEffect(() => {
    if (!id || confirmationStarted.current) {
      return;
    }

    confirmationStarted.current = true;
    const orderId = Number(id);

    const cartCleanup = user?.id ? deleteCart(user.id) : Promise.resolve();

    cartCleanup
      .then(() => validateStripeSession(orderId))
      .then(() => {
        toast.success('Order has been placed');
      })
      .finally(() => setLoading(false));
  }, [id]);

  if (loading) {
    return <Loader message="Confirming order..." />;
  }

  return (
    <section className="surface confirmation">
      <h1>Thank you for your order</h1>
      <p>Order #{id} has been confirmed.</p>
      <Link className="btn btn-primary" to="/">
        Continue Shopping
      </Link>
    </section>
  );
}
