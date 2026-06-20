import { Component } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ProductService } from '../../services/ProductService';
import { ResponseDto } from '../../dto/ResponseDto';
import { Observable } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/AuthenticationService';
import { ShoppingCart } from '../../dto/ShoppingCart';
import { ShoppingCartHeader } from '../../dto/ShoppingCartHeader';
import { ShoppingCartDetail } from '../../dto/ShoppingCartDetail';
import { ShoppingCartService } from '../../services/ShoppingCartService';
import { CouponService } from '../../services/CouponService';
import { Router, RouterLink } from '@angular/router';
import {
  FormsModule,
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-shopping-cart-component',
  imports: [CurrencyPipe, FormsModule, ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './shopping-cart-component.html',
  styleUrl: './shopping-cart-component.css',
  providers: [ProductService, ShoppingCartService, CouponService],
})
export class ShoppingCartComponent {
  private shoppingCart$: Observable<ResponseDto> | null= null;
  public shoppingCartForm: FormGroup | any = null;
  public shoppingCart: ShoppingCart | any = null;
  private response: ResponseDto | any = null;
  public loading : boolean = false;
  public shoppingCartIsEmpty : boolean = false;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private shoppingCartService: ShoppingCartService,
    private toastr: ToastrService,
    private authService: AuthService,
    private couponService : CouponService,
    private router: Router,
  ) {
    this.reloadShoppingCart();
  }

  reloadShoppingCart() {
    this.shoppingCartIsEmpty = false;
    this.loading = true;
    this.shoppingCart$ = this.shoppingCartService.getShoppingCart();
    this.shoppingCart$.subscribe({
      next: (responseDto) => {
        this.response = responseDto;
      },
      error: (err) => {
        console.error('Error', err);
        this.shoppingCart = null;
      },
      complete: () => {
        console.log('Done');
        this.shoppingCart = this.response.result;
        if (this.shoppingCart == null || this.shoppingCart.cartHeader == null) {
          this.shoppingCartIsEmpty = true;
        }
        this.loading = false;
        if (this.shoppingCart == null) {
          this.toastr.error(`No shopping cart for the currently logged on user has been found`);
        }
      },
    });
 }

 onDeleteProductFromCart(orderDetailsId: number) {
  console.log("Removing product from shopping cart");
  this.shoppingCart$ = this.shoppingCartService.removeProductFromCart(orderDetailsId);
  this.shoppingCart$.subscribe({
    next: responseDto => {
      this.response = responseDto;
      if (!this.response.isSuccess) {
        throw new Error(this.response.message);
      }
      this.reloadShoppingCart();
    }
  })
}

  removeCoupon() {
    this.shoppingCart$ = this.shoppingCartService.removeCoupon();
    this.shoppingCart$.subscribe({
      next: () => {
        console.log('Done');
        this.reloadShoppingCart();
      }
    });
  }

  applyCoupon(couponCode : string) {
    this.shoppingCart$ = this.shoppingCartService.applyCoupon(couponCode);
    this.shoppingCart$.subscribe({
      next: () => {
        console.log('Done');
        this.reloadShoppingCart();
      }
    });
  }

  emptyCart() {
    this.shoppingCart$ = this.shoppingCartService.emptyCart();
    this.shoppingCart$.subscribe({
      next: () => {
        console.log('Shopping cart has been emptied');
        this.router.navigate(['/']);
      }
    });
  }
}
