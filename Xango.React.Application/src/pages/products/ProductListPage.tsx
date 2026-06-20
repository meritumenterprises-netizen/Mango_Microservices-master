import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import Swal from 'sweetalert2';
import { toast } from 'react-toastify';
import { Loader } from '../../components/Loader';
import { PageTitle } from '../../components/PageTitle';
import { deleteProduct, getProducts } from '../../services/productService';
import type { Product } from '../../types';
import { currency, productImage, truncate } from '../../utils/format';

export function ProductListPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);

  async function load() {
    setLoading(true);
    try {
      setProducts(await getProducts());
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    load();
  }, []);

  async function onDelete(product: Product) {
    const result = await Swal.fire({
      title: 'Delete product?',
      text: product.name,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Delete'
    });
    if (!result.isConfirmed) {
      return;
    }
    await deleteProduct(product.productId);
    toast.success('Product deleted');
    load();
  }

  if (loading) {
    return <Loader message="Loading products..." />;
  }

  return (
    <>
      <PageTitle
        title="Products"
        action={
          <Link className="btn btn-outline-primary" to="/product/create">
            Create New Product
          </Link>
        }
      />
      <div className="table-responsive surface">
        <table className="table align-middle mb-0">
          <thead>
            <tr>
              <th>Product</th>
              <th>Category</th>
              <th>Price</th>
              <th>Items in Stock</th>
              <th className="text-end">Actions</th>
            </tr>
          </thead>
          <tbody>
            {products.map((product) => (
              <tr key={product.productId}>
                <td>
                  <div className="table-product">
                    <img src={productImage(product)} alt={product.name} />
                    <div>
                      <strong>{product.name}</strong>
                      <p>{truncate(product.description, 80)}</p>
                    </div>
                  </div>
                </td>
                <td>
                  <span className="category-badge">{product.categoryName}</span>
                </td>
                <td>{currency(product.price)}</td>
                <td>
                  <span className={`stock-badge ${product.stockInventory <= 0 ? 'out-of-stock' : ''}`}>
                    {product.stockInventory} items
                  </span>
                </td>
                <td className="text-end">
                  <Link className="btn btn-sm btn-secondary me-2" to={`/product/details/${product.productId}`}>
                    Details
                  </Link>
                  <Link className="btn btn-sm btn-primary me-2" to={`/product/edit/${product.productId}`}>
                    Edit
                  </Link>
                  <button className="btn btn-sm btn-danger" onClick={() => onDelete(product)}>
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
