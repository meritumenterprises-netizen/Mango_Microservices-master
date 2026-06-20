import { FormEvent, useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Loader } from '../../components/Loader';
import { createProduct, getProduct, updateProduct } from '../../services/productService';
import type { Product } from '../../types';
import { productImage } from '../../utils/format';

const emptyProduct: Product = {
  productId: 0,
  name: '',
  price: 0,
  description: '',
  categoryName: '',
  base64Image: null,
  imageUrl: null,
  imageLocalPath: null,
  count: 1,
  stockInventory: 0
};

export function ProductFormPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [product, setProduct] = useState<Product>(emptyProduct);
  const [loading, setLoading] = useState(Boolean(id));
  const [dirty, setDirty] = useState(false);

  useEffect(() => {
    if (!id) {
      return;
    }
    getProduct(Number(id))
      .then(setProduct)
      .finally(() => setLoading(false));
  }, [id]);

  async function readImage(file: File) {
    const dataUrl = await new Promise<string>((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve(String(reader.result));
      reader.onerror = reject;
      reader.readAsDataURL(file);
    });
    setDirty(true);
    setProduct({ ...product, base64Image: dataUrl, imageUrl: null });
  }

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    if (id) {
      await updateProduct(product);
      toast.success('Product successfully updated');
    } else {
      await createProduct(product);
      toast.success('Product successfully created');
    }
    setDirty(false);
    navigate('/product');
  }

  useEffect(() => {
    const handler = (event: BeforeUnloadEvent) => {
      if (dirty) {
        event.preventDefault();
      }
    };
    window.addEventListener('beforeunload', handler);
    return () => window.removeEventListener('beforeunload', handler);
  }, [dirty]);

  if (loading) {
    return <Loader message="Loading product..." />;
  }

  return (
    <form className="surface form-page" onSubmit={onSubmit}>
      <h1>{id ? 'Edit Product' : 'Create Product'}</h1>
      <div className="product-edit-grid">
        <div className="form-stack">
          <input
            className="form-control"
            placeholder="Name"
            value={product.name}
            onChange={(e) => {
              setDirty(true);
              setProduct({ ...product, name: e.target.value });
            }}
            required
          />
          <input
            className="form-control"
            placeholder="Category"
            value={product.categoryName}
            onChange={(e) => {
              setDirty(true);
              setProduct({ ...product, categoryName: e.target.value });
            }}
            required
          />
          <div className="row g-2">
            <div className="col-md-6">
              <input
                className="form-control"
                type="number"
                min="0"
                step="0.01"
                placeholder="Price"
                value={product.price}
                onChange={(e) => {
                  setDirty(true);
                  setProduct({ ...product, price: Number(e.target.value) });
                }}
                required
              />
            </div>
            <div className="col-md-6">
              <input
                className="form-control"
                type="number"
                min="0"
                placeholder="Stock inventory"
                value={product.stockInventory}
                onChange={(e) => {
                  setDirty(true);
                  setProduct({ ...product, stockInventory: Number(e.target.value) });
                }}
                required
              />
            </div>
          </div>
          <textarea
            className="form-control"
            rows={6}
            placeholder="Description"
            value={product.description}
            onChange={(e) => {
              setDirty(true);
              setProduct({ ...product, description: e.target.value });
            }}
            required
          />
          <input className="form-control" type="file" accept="image/*" onChange={(e) => e.target.files?.[0] && readImage(e.target.files[0])} />
        </div>
        <img className="edit-preview" src={productImage(product)} alt={product.name || 'Product preview'} />
      </div>
      <div className="d-flex gap-2">
        <button className="btn btn-primary" type="submit">
          Save
        </button>
        <Link className="btn btn-outline-secondary" to="/product">
          Back to products
        </Link>
      </div>
    </form>
  );
}
