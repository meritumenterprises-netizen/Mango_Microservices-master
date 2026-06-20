import { apiClient, showApiError, unwrapResponse } from './apiClient';
import { serviceSettings } from '../config/serviceSettings';
import { getUser } from './authStorage';
import type { ResponseDto, ShoppingCart } from '../types';

function requireUserId() {
  const user = getUser();
  if (!user) {
    throw new Error('Cannot access a shopping cart if a user is not logged in');
  }
  return user.id;
}

export async function getShoppingCart() {
  const userId = requireUserId();
  try {
    const { data } = await apiClient.get<ResponseDto<ShoppingCart>>(
      `${serviceSettings.SHOPPINGCART_API}/api/cart/GetCart/${userId}`
    );
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error loading shopping cart');
  }
}

export async function applyCoupon(couponCode: string) {
  const userId = requireUserId();
  try {
    const { data } = await apiClient.post<ResponseDto>(
      `${serviceSettings.SHOPPINGCART_API}/api/cart/ApplyCouponToCart`,
      { userId, couponCode }
    );
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error applying coupon');
  }
}

export async function removeCoupon() {
  const userId = requireUserId();
  try {
    const { data } = await apiClient.post<ResponseDto>(
      `${serviceSettings.SHOPPINGCART_API}/api/cart/RemoveCoupon`,
      { userId }
    );
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error removing coupon');
  }
}

export async function addProductToCart(productId: number, quantity: number, stockQuantity: number) {
  const userId = requireUserId();
  try {
    const { data } = await apiClient.post<ResponseDto>(
      `${serviceSettings.SHOPPINGCART_API}/api/cart/AddProductToCart`,
      { userId, productId, quantity, stockQuantity }
    );
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Cannot add product to cart');
  }
}

export async function removeProductFromCart(cartDetailId: number) {
  try {
    const { data } = await apiClient.post<ResponseDto>(
      `${serviceSettings.SHOPPINGCART_API}/api/cart/RemoveProductFromCart`,
      { cartDetailsId: cartDetailId.toString() }
    );
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error removing product from cart');
  }
}

export async function emptyCart() {
  const userId = requireUserId();
  return deleteCart(userId);
}

export async function deleteCart(userId: string) {
  try {
    const { data } = await apiClient.delete<ResponseDto>(
      `${serviceSettings.SHOPPINGCART_API}/api/cart/DeleteCart/${userId}`
    );
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error emptying cart');
  }
}
