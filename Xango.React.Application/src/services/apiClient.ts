import axios, { AxiosError } from 'axios';
import { toast } from 'react-toastify';
import type { ResponseDto } from '../types';
import { getToken } from './authStorage';

export const apiClient = axios.create();

apiClient.interceptors.request.use((config) => {
  const token = getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export function unwrapResponse<T>(response: ResponseDto<T>): T {
  if (!response.isSuccess) {
    throw new Error(response.message || 'Request failed');
  }
  return response.result;
}

export function showApiError(error: unknown, fallback: string): never {
  const axiosError = error as AxiosError<ResponseDto>;
  const message =
    axiosError.response?.data?.message ||
    axiosError.message ||
    (error instanceof Error ? error.message : fallback);
  toast.error(message);
  throw error;
}
