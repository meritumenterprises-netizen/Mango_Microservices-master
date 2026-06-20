export function currency(value?: number | null) {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value ?? 0);
}

export function splitCamelCase(value?: string | null) {
  const normalized = value?.toLowerCase() === 'complete' ? 'completed' : value;
  return (normalized ?? '').replace(/([a-z])([A-Z])/g, '$1 $2');
}

export function truncate(value?: string | null, length = 100) {
  if (!value) {
    return '';
  }
  return value.length > length ? `${value.slice(0, length)}...` : value;
}

export function productImage(product: { base64Image?: string | null; imageUrl?: string | null }) {
  if (product.base64Image) {
    return product.base64Image.startsWith('data:')
      ? product.base64Image
      : `data:image/png;base64,${product.base64Image}`;
  }
  return product.imageUrl || '/vite.svg';
}
