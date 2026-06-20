import { FormEvent, useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Loader } from '../../components/Loader';
import { createCoupon, getCoupon, updateCoupon } from '../../services/couponService';
import type { Coupon } from '../../types';

const emptyCoupon: Coupon = {
  couponId: 0,
  couponCode: '',
  discountAmount: 0,
  minAmount: 0
};

export function CouponFormPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [coupon, setCoupon] = useState<Coupon>(emptyCoupon);
  const [loading, setLoading] = useState(Boolean(id));
  const [dirty, setDirty] = useState(false);

  useEffect(() => {
    if (!id) {
      return;
    }
    getCoupon(Number(id))
      .then(setCoupon)
      .finally(() => setLoading(false));
  }, [id]);

  useEffect(() => {
    const handler = (event: BeforeUnloadEvent) => {
      if (dirty) {
        event.preventDefault();
      }
    };
    window.addEventListener('beforeunload', handler);
    return () => window.removeEventListener('beforeunload', handler);
  }, [dirty]);

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    if (id) {
      await updateCoupon(coupon);
      toast.success('Coupon successfully updated');
    } else {
      await createCoupon(coupon);
      toast.success('Coupon successfully created');
    }
    setDirty(false);
    navigate('/coupon');
  }

  if (loading) {
    return <Loader message="Loading coupon..." />;
  }

  return (
    <form className="surface form-page" onSubmit={onSubmit}>
      <h1>{id ? 'Edit Coupon' : 'Create Coupon'}</h1>
      <input
        className="form-control"
        placeholder="Coupon code"
        value={coupon.couponCode}
        onChange={(e) => {
          setDirty(true);
          setCoupon({ ...coupon, couponCode: e.target.value });
        }}
        required
      />
      <input
        className="form-control"
        type="number"
        min="0"
        step="0.01"
        placeholder="Discount amount"
        value={coupon.discountAmount}
        onChange={(e) => {
          setDirty(true);
          setCoupon({ ...coupon, discountAmount: Number(e.target.value) });
        }}
        required
      />
      <input
        className="form-control"
        type="number"
        min="0"
        step="0.01"
        placeholder="Minimum amount"
        value={coupon.minAmount}
        onChange={(e) => {
          setDirty(true);
          setCoupon({ ...coupon, minAmount: Number(e.target.value) });
        }}
        required
      />
      <div className="d-flex gap-2">
        <button className="btn btn-primary" type="submit">
          Save
        </button>
        <Link className="btn btn-outline-secondary" to="/coupon">
          Back to List
        </Link>
      </div>
    </form>
  );
}
