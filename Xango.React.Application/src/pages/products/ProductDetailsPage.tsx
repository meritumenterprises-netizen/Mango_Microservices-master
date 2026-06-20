import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Loader } from '../../components/Loader';
import { addProductToCart } from '../../services/cartService';
import { getProduct } from '../../services/productService';
import { useAuth } from '../../state/AuthContext';
import type { Product } from '../../types';
import { currency, productImage } from '../../utils/format';

export function ProductDetailsPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { isLoggedIn } = useAuth();
  const [product, setProduct] = useState<Product | null>(null);
  const [quantity, setQuantity] = useState(1);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getProduct(Number(id))
      .then(setProduct)
      .finally(() => setLoading(false));
  }, [id]);

  async function onAddToCart() {
    if (!product) {
      return;
    }
    if (!isLoggedIn) {
      navigate('/login', { state: { from: `/product/details/${product.productId}` } });
      return;
    }
    await addProductToCart(product.productId, quantity, product.stockInventory);
    toast.success('Product added to cart');
    navigate('/cart');
  }

  if (loading || !product) {
    return <Loader message="Loading product..." />;
  }

  return (
    <section className="surface product-detail">
      <img src={productImage(product)} alt={product.name} />
      <div>
        <p className="text-uppercase text-muted small">{product.categoryName}</p>
        <h1>{product.name}</h1>
        <p>{product.description}</p>
        <p className="detail-price">{currency(product.price)}</p>
        <span className={`stock-badge detail-stock ${product.stockInventory <= 0 ? 'out-of-stock' : ''}`}>
          {product.stockInventory} items in stock
        </span>
        <div className="detail-actions">
          <input
            className="form-control"
            type="number"
            min="1"
            max={Math.max(product.stockInventory, 1)}
            value={quantity}
            onChange={(e) => setQuantity(Number(e.target.value))}
          />
          <button className="btn btn-primary" onClick={onAddToCart} disabled={product.stockInventory <= 0}>
            Add to Cart
          </button>
          <Link className="btn btn-outline-secondary" to="/">
            Back
          </Link>
        </div>
      </div>
    </section>
  );
}
