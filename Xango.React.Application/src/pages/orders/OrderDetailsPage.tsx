import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import Swal from 'sweetalert2';
import { toast } from 'react-toastify';
import { Loader } from '../../components/Loader';
import { deleteOrder, getOrder, updateOrderStatus } from '../../services/orderService';
import type { OrderHeader } from '../../types';
import { currency, splitCamelCase } from '../../utils/format';

export function OrderDetailsPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [order, setOrder] = useState<OrderHeader | null>(null);
  const [loading, setLoading] = useState(true);

  async function load() {
    setLoading(true);
    try {
      setOrder(await getOrder(Number(id)));
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    load();
  }, [id]);

  async function setStatus(status: string) {
    await updateOrderStatus(Number(id), status);
    toast.success('Order status updated');
    load();
  }

  async function onDelete() {
    const result = await Swal.fire({
      title: 'Delete order?',
      text: `Order #${id}`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Delete'
    });
    if (!result.isConfirmed) {
      return;
    }
    await deleteOrder(Number(id));
    toast.success(`Order #${id} has been deleted`);
    navigate('/order');
  }

  if (loading || !order) {
    return <Loader message="Loading order..." />;
  }

  return (
    <section className="surface order-detail">
      <div className="page-title">
        <h1>Order #{order.orderHeaderId}</h1>
        <Link className="btn btn-outline-secondary" to="/order">
          Back to Orders
        </Link>
      </div>
      <div className="order-meta">
        <div>
          <span>Customer</span>
          <strong>{order.name}</strong>
          <p>{order.email}</p>
          <p>{order.phone}</p>
        </div>
        <div>
          <span>Status</span>
          <strong>{splitCamelCase(order.status)}</strong>
        </div>
        <div>
          <span>Total</span>
          <strong>{currency(order.orderTotal)}</strong>
        </div>
      </div>
      <div className="table-responsive">
        <table className="table align-middle">
          <thead>
            <tr>
              <th>Product</th>
              <th>Price</th>
              <th>Count</th>
              <th>Total</th>
            </tr>
          </thead>
          <tbody>
            {order.orderDetails?.map((detail) => (
              <tr key={detail.orderDetailsId}>
                <td>{detail.productName || detail.product?.name}</td>
                <td>{currency(detail.price)}</td>
                <td>{detail.count}</td>
                <td>{currency(detail.price * detail.count)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <div className="detail-actions flex-wrap">
        <button className="btn btn-outline-primary" onClick={() => setStatus('Approved')}>
          Approve
        </button>
        <button className="btn btn-outline-primary" onClick={() => setStatus('ReadyForPickup')}>
          Ready For Pickup
        </button>
        <button className="btn btn-outline-primary" onClick={() => setStatus('Completed')}>
          Completed
        </button>
        <button className="btn btn-outline-primary" onClick={() => setStatus('Shipped')}>
          Shipped
        </button>
        <button className="btn btn-outline-danger" onClick={() => setStatus('Cancelled')}>
          Cancel
        </button>
        <button className="btn btn-danger" onClick={onDelete}>
          Delete
        </button>
      </div>
    </section>
  );
}
