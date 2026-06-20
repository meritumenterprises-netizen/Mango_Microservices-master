import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Loader } from '../../components/Loader';
import { PageTitle } from '../../components/PageTitle';
import { getOrders } from '../../services/orderService';
import { useAuth } from '../../state/AuthContext';
import type { OrderHeader } from '../../types';
import { currency, splitCamelCase } from '../../utils/format';

const statuses = ['pending', 'approved', 'readyforpickup', 'cancelled', 'completed', 'shipped', 'all'];

export function OrdersPage() {
  const { status } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [orders, setOrders] = useState<OrderHeader[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!user) {
      return;
    }
    setLoading(true);
    getOrders(user.id, status)
      .then(setOrders)
      .finally(() => setLoading(false));
  }, [status, user]);

  if (loading) {
    return <Loader message="Loading orders..." />;
  }

  return (
    <>
      <PageTitle title="Orders" />
      <div className="status-tabs">
        {statuses.map((item) => (
          <button
            className={`btn btn-sm ${
              status === item || (!status && item === 'all') ? 'btn-primary' : 'btn-outline-primary'
            }`}
            key={item}
            onClick={() => navigate(item === 'all' ? '/order/all' : `/order/${item}`)}
          >
            {splitCamelCase(item)}
          </button>
        ))}
      </div>
      <div className="table-responsive surface">
        <table className="table align-middle mb-0">
          <thead>
            <tr>
              <th>Order</th>
              <th>Customer</th>
              <th>Status</th>
              <th>Total</th>
              <th className="text-end">Actions</th>
            </tr>
          </thead>
          <tbody>
            {orders.map((order) => (
              <tr key={order.orderHeaderId}>
                <td>#{order.orderHeaderId}</td>
                <td>{order.name || order.email || order.userEmail}</td>
                <td>{splitCamelCase(order.status)}</td>
                <td>{currency(order.orderTotal)}</td>
                <td className="text-end">
                  <button className="btn btn-sm btn-primary" onClick={() => navigate(`/order/details/${order.orderHeaderId}`)}>
                    Details
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}
