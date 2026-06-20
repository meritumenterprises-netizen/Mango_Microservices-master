export interface ResponseDto<T = unknown> {
  result: T;
  isSuccess: boolean;
  message: string;
  stackTrace?: string;
}

export interface Coupon {
  couponId: number;
  couponCode: string;
  discountAmount: number;
  minAmount: number;
}

export interface Product {
  productId: number;
  name: string;
  price: number;
  description: string;
  categoryName: string;
  base64Image?: string | null;
  imageUrl?: string | null;
  imageLocalPath?: string | null;
  count: number;
  stockInventory: number;
}

export interface RegistrationRequest {
  email: string;
  name: string;
  phoneNumber: string;
  password: string;
  role: string;
}

export interface UserRecord {
  id: string;
  email: string;
  name: string;
  phoneNumber: string;
}

export interface UserToken {
  token: string;
}

export interface UserResponse {
  user: UserRecord;
  token?: string;
  userToken?: UserToken;
}

export interface ShoppingCartHeader {
  cartHeaderId: number;
  userId?: string;
  couponCode?: string;
  discount: number;
  cartTotal: number;
  name: string;
  phone: string;
  email: string;
}

export interface ShoppingCartDetail {
  cartDetailsId: number;
  cartHeaderId: number;
  cartHeader?: ShoppingCartHeader;
  productId: number;
  product?: Product;
  count: number;
}

export interface ShoppingCart {
  cartHeader: ShoppingCartHeader;
  cartDetails?: ShoppingCartDetail[];
}

export interface OrderDetail {
  orderDetailsId: number;
  orderHeaderId: number;
  productId: number;
  product?: Product;
  productName: string;
  price: number;
  count: number;
}

export interface OrderHeader {
  orderHeaderId: number;
  userId: string;
  couponCode: string;
  discount: number;
  orderTotal: number;
  orderTotalWithCurrency: string;
  name: string;
  phone: string;
  email: string;
  userEmail: string;
  orderTime: string;
  modifiedTime: string;
  status: string;
  paymentIntentId?: string;
  stripeSessionId?: string;
  orderDetails: OrderDetail[];
}

export interface StripeRequest {
  stripeSessionUrl?: string | null;
  stripeSessionId?: string | null;
  approvedUrl?: string;
  cancelUrl?: string;
  orderHeader?: OrderHeader | null;
}
