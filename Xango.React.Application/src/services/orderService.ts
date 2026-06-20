import { apiClient, showApiError, unwrapResponse } from './apiClient';
import { serviceSettings } from '../config/serviceSettings';
import type { OrderHeader, ResponseDto, ShoppingCart, StripeRequest } from '../types';

export async function createOrder(shoppingCart: ShoppingCart) {
  try {
    const { data } = await apiClient.post<ResponseDto<OrderHeader>>(
      `${serviceSettings.ORDER_API}/api/order/CreateOrder`,
      shoppingCart
    );
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error creating order');
  }
}

export async function createStripeSession(stripeRequest: StripeRequest) {
  try {
    const { data } = await apiClient.post<ResponseDto<StripeRequest>>(
      `${serviceSettings.ORDER_API}/api/order/CreateStripeSession`,
      stripeRequest
    );
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error creating Stripe session');
  }
}

export async function validateStripeSession(orderId: number) {
  try {
    const { data } = await apiClient.post<ResponseDto>(
      `${serviceSettings.ORDER_API}/api/order/ValidateStripeSession`,
      JSON.stringify(orderId),
      { headers: { 'Content-Type': 'application/json' } }
    );
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error validating Stripe session');
  }
}

export async function getOrders(userId: string, status?: string) {
  const orderStatus = status ?? 'all';
  try {
    const { data } = await apiClient.get<ResponseDto<OrderHeader[]>>(
      `${serviceSettings.ORDER_API}/api/order/GetAll`,
      { params: { status: orderStatus, userId } }
    );
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error loading orders');
  }
}

export async function getOrder(orderId: number) {
  try {
    const { data } = await apiClient.get<ResponseDto<OrderHeader>>(
      `${serviceSettings.ORDER_API}/api/order/GetOrder/${orderId}`
    );
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error loading order');
  }
}

export async function updateOrderStatus(orderId: number, newStatus: string) {
  try {
    const { data } = await apiClient.post<ResponseDto>(
      `${serviceSettings.ORDER_API}/api/order/UpdateOrderStatus/${orderId}`,
      JSON.stringify(newStatus),
      { headers: { 'Content-Type': 'application/json' } }
    );
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error updating order status');
  }
}

export async function deleteOrder(orderId: number) {
  try {
    const { data } = await apiClient.delete<ResponseDto>(
      `${serviceSettings.ORDER_API}/api/order/DeleteOrder/${orderId}`
    );
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error deleting order');
  }
}
