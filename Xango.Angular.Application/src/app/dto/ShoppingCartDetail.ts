import { ShoppingCartHeader } from "./ShoppingCartHeader";
import { Product } from "./Product";

export interface ShoppingCartDetail {
  cartDetailsId : number;
  cartHeaderId: number;
  cartHeader?: ShoppingCartHeader;
  productId : number;
  product?: Product;
  count: number;
}

