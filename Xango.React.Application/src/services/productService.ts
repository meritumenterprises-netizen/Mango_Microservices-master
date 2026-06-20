import { apiClient, showApiError, unwrapResponse } from './apiClient';
import { serviceSettings } from '../config/serviceSettings';
import type { Product, ResponseDto } from '../types';

export async function getProducts() {
  try {
    const { data } = await apiClient.get<ResponseDto<Product[]>>(`${serviceSettings.PRODUCT_API}/api/product`);
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error loading products');
  }
}

export async function getProduct(productId: number) {
  try {
    const { data } = await apiClient.get<ResponseDto<Product>>(
      `${serviceSettings.PRODUCT_API}/api/product/${productId}`
    );
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, `Error loading product ${productId}`);
  }
}

export async function createProduct(product: Product) {
  try {
    const { data } = await apiClient.post<ResponseDto<Product>>(`${serviceSettings.PRODUCT_API}/api/product/`, product);
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error creating product');
  }
}

export async function updateProduct(product: Product) {
  try {
    const { data } = await apiClient.put<ResponseDto<Product>>(`${serviceSettings.PRODUCT_API}/api/product`, product);
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, `Error updating product ${product.productId}`);
  }
}

export async function deleteProduct(productId: number) {
  try {
    const { data } = await apiClient.delete<ResponseDto>(`${serviceSettings.PRODUCT_API}/api/product/${productId}`);
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error deleting product');
  }
}
