import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Loader } from '../components/Loader';
import { getProducts } from '../services/productService';
import type { Product } from '../types';
import { currency, productImage, truncate } from '../utils/format';

export function HomePage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getProducts()
      .then(setProducts)
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return <Loader message="Loading products..." />;
  }

  return (
    <div className="product-grid">
      {products.map((product) => (
        <article className="product-card" key={product.productId}>
          <img src={productImage(product)} alt={product.name} />
          <div className="product-card-body">
            <div>
              <span className="category-badge">{product.categoryName}</span>
              <h2>{product.name}</h2>
              <p>{truncate(product.description, 120)}</p>
              <span className={`stock-badge ${product.stockInventory <= 0 ? 'out-of-stock' : ''}`}>
                {product.stockInventory} items in stock
              </span>
            </div>
            <div className="product-card-footer">
              <strong>{currency(product.price)}</strong>
              <Link className="btn btn-outline-primary btn-sm" to={`/product/details/${product.productId}`}>
                Details
              </Link>
            </div>
          </div>
        </article>
      ))}
    </div>
  );
}
