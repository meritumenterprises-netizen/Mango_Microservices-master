import { apiClient, showApiError, unwrapResponse } from './apiClient';
import { serviceSettings } from '../config/serviceSettings';
import type { RegistrationRequest, ResponseDto, UserRecord, UserResponse } from '../types';

export async function login(userName: string, password: string) {
  try {
    const { data } = await apiClient.post<ResponseDto<UserResponse>>(
      `${serviceSettings.AUTH_API}/api/auth/login`,
      { userName, password }
    );
    const result = unwrapResponse(data);
    const token = result.token ?? result.userToken?.token;
    if (!token) {
      throw new Error('Login response did not include a token');
    }
    return { token, user: result.user };
  } catch (error) {
    showApiError(error, 'Login failed');
  }
}

export async function register(registrationRequest: RegistrationRequest) {
  try {
    const { data } = await apiClient.post<ResponseDto>(
      `${serviceSettings.AUTH_API}/api/auth/register`,
      registrationRequest
    );
    return unwrapResponse(data);
  } catch (error) {
    showApiError(error, 'Registration failed');
  }
}

export async function isAdmin(user: UserRecord | null) {
  if (!user) {
    return false;
  }
  try {
    const { data } = await apiClient.get<ResponseDto<boolean>>(
      `${serviceSettings.AUTH_API}/api/auth/InRole/${user.email}/ADMIN`
    );
    return unwrapResponse(data);
  } catch {
    return false;
  }
}
