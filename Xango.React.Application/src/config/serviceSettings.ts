const env = import.meta.env;

export const serviceSettings = {
  AUTH_API: env.VITE_AUTH_API ?? 'http://auth-api.default.svc.cluster.local',
  PRODUCT_API: env.VITE_PRODUCT_API ?? 'http://product-api.default.svc.cluster.local',
  COUPON_API: env.VITE_COUPON_API ?? 'http://coupon-api.default.svc.cluster.local',
  SHOPPINGCART_API: env.VITE_SHOPPINGCART_API ?? 'http://shoppingcart-api.default.svc.cluster.local',
  ORDER_API: env.VITE_ORDER_API ?? 'http://order-api.default.svc.cluster.local',
  INVENTORY_API: env.VITE_INVENTORY_API ?? 'http://inventory-api.default.svc.cluster.local'
};
