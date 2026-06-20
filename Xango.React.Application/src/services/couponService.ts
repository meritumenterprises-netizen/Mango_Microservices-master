import { apiClient, showApiError, unwrapResponse } from './apiClient';
import { serviceSettings } from '../config/serviceSettings';
import type { Coupon, ResponseDto } from '../types';

export async function getCoupons() {
  try {
    const { data } = await apiClient.get<ResponseDto<Coupon[]>>(`${serviceSettings.COUPON_API}/api/coupon`);
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error loading coupons');
  }
}

export async function getCoupon(id: number) {
  try {
    const { data } = await apiClient.get<ResponseDto<Coupon>>(`${serviceSettings.COUPON_API}/api/coupon/${id}`);
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error loading coupon');
  }
}

export async function getCouponByCode(code: string) {
  try {
    const { data } = await apiClient.get<ResponseDto<Coupon>>(
      `${serviceSettings.COUPON_API}/api/coupon/GetByCode/${code}`
    );
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error loading coupon');
  }
}

export async function createCoupon(coupon: Coupon) {
  try {
    const { data } = await apiClient.post<ResponseDto<Coupon>>(`${serviceSettings.COUPON_API}/api/coupon`, coupon);
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error creating coupon');
  }
}

export async function updateCoupon(coupon: Coupon) {
  try {
    const { data } = await apiClient.put<ResponseDto<Coupon>>(`${serviceSettings.COUPON_API}/api/coupon`, coupon);
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error updating coupon');
  }
}

export async function deleteCoupon(id: number) {
  try {
    const { data } = await apiClient.delete<ResponseDto>(`${serviceSettings.COUPON_API}/api/coupon/${id}`);
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Error deleting coupon');
  }
}
