import { ShoppingCartHeader } from "./ShoppingCartHeader";
import { ShoppingCartDetail } from "./ShoppingCartDetail";

export interface ShoppingCart {
  cartHeader: ShoppingCartHeader;
  cartDetails?: ShoppingCartDetail[] | any;
}

