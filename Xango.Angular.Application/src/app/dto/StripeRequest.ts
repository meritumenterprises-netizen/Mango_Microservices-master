import { OrderHeader } from "./OrderHeader";
import { OrderDetail } from "./OrderDetail";

export class StripeRequest {
	stripeSessionUrl?: string | null = null;
	stripeSessionId?: string | null = null;
	approvedUrl?: string;
	cancelUrl?: string;
	orderHeader? : OrderHeader | null  = null;
  }
  
  