import { OrderDetail } from "./OrderDetail";

export interface OrderHeader {
	orderHeaderId : number;
	userId : string;
	couponCode : string;
	discount: number;
	orderTotal: number;
	orderTotalWithCurrency: string;
	name: string;
	phone: string;
	email: string;  
	userEmail: string;
	orderTime: Date;
	modifiedTime: Date;
	status: string;
	paymentIntentId?: string;
	stripeSessionId?: string;
	orderDetails: OrderDetail[];
  }
  
  