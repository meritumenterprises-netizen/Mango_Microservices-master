import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import Swal from 'sweetalert2';
import { toast } from 'react-toastify';
import { Loader } from '../../components/Loader';
import { PageTitle } from '../../components/PageTitle';
import { deleteCoupon, getCoupons } from '../../services/couponService';
import type { Coupon } from '../../types';
import { currency } from '../../utils/format';

export function CouponListPage() {
  const [coupons, setCoupons] = useState<Coupon[]>([]);
  const [loading, setLoading] = useState(true);

  async function load() {
    setLoading(true);
    try {
      setCoupons(await getCoupons());
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    load();
  }, []);

  async function onDelete(coupon: Coupon) {
    const result = await Swal.fire({
      title: 'Delete coupon?',
      text: coupon.couponCode,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Delete'
    });
    if (!result.isConfirmed) {
      return;
    }
    await deleteCoupon(coupon.couponId);
    toast.success('Coupon deleted');
    load();
  }

  if (loading) {
    return <Loader message="Loading coupons..." />;
  }

  return (
    <>
      <PageTitle
        title="Coupons"
        action={
          <Link className="btn btn-outline-primary" to="/coupon/create">
            Create New Coupon
          </Link>
        }
      />
      <div className="table-responsive surface">
        <table className="table align-middle mb-0">
          <thead>
            <tr>
              <th>Code</th>
              <th>Discount</th>
              <th>Minimum Amount</th>
              <th className="text-end">Actions</th>
            </tr>
          </thead>
          <tbody>
            {coupons.map((coupon) => (
              <tr key={coupon.couponId}>
                <td>{coupon.couponCode}</td>
                <td>{currency(coupon.discountAmount)}</td>
                <td>{currency(coupon.minAmount)}</td>
                <td className="text-end">
                  <Link className="btn btn-sm btn-primary me-2" to={`/coupon/edit/${coupon.couponId}`}>
                    Edit
                  </Link>
                  <button className="btn btn-sm btn-danger" onClick={() => onDelete(coupon)}>
                    Delete
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
