import { Injectable } from '@angular/core';
import { HttpClient, HttpHandler } from '@angular/common/http';
import { Observable, catchError, concatMap, of, switchMap, tap } from 'rxjs';
import { ServiceSettings } from './ServiceSettings';
import { ResponseDto } from '../dto/ResponseDto';
import { AppComponent } from '../app.component';
import { ToastrService } from 'ngx-toastr';
import { Product } from '../dto/Product';
import { UserRecord } from '../dto/UserRecord';
import { AuthService } from './AuthenticationService';
import { Coupon } from '../dto/Coupon';
import { CouponService } from './CouponService';
import { OrderHeader } from '../dto/OrderHeader';
import { firstValueFrom } from 'rxjs';
import { ApplyCouponDto } from '../dto/ApplyCoupon';
import { RemoveCouponDto } from '../dto/RemoveCoupon';
import { AddProductToCartDto } from '../dto/AddProductToCart';
import { RemoveProductFromCartDto } from '../dto/RemoveProductFromCart';

@Injectable({
  providedIn: 'root',
})
export class ShoppingCartService {
  private readonly baseUrl = ServiceSettings.SHOPPINGCART_API;

  private coupon$: Observable<ResponseDto> | any;

  constructor(
    private http: HttpClient,
    private toastr: ToastrService,
    private authService: AuthService,
    private couponService: CouponService,
  ) {
    toastr.toastrConfig.timeOut = 5000;
    toastr.toastrConfig.closeButton = true;
    toastr.toastrConfig.enableHtml = true;
  }

  public getShoppingCart() {
    let user: UserRecord | null = this.authService.getUser();
    if (user == null) {
      throw new Error('Cannot get a shopping cart if a user is not logged in');
    }
    return this.http.get<ResponseDto>(`${this.baseUrl}/api/cart/GetCart/${user.id}`).pipe(
      tap((responseDto) => {
        if (!responseDto.isSuccess) {
          throw new Error(responseDto.message);
        }
      }),
      catchError((err) => {
        this.toastr.error(`Error loading shopping cart for user id ${user.id}}\r\n` + err.message, 'Error');
        throw err;
      }),
    );
  }

  public removeCoupon() {
    let user: UserRecord | null = this.authService.getUser();
    if (user == null) {
      throw new Error('Cannot get a shopping cart if a user is not logged in');
    }
    let removeCouponDto: RemoveCouponDto = { userId: user.id };
    return this.http
      .post<ResponseDto>(`${this.baseUrl}/api/cart/RemoveCoupon`, removeCouponDto)
      .pipe(
        tap((responseDto) => {
          if (!responseDto.isSuccess) {
            throw new Error(responseDto.message);
          }
        }),
        catchError((err) => {
          this.toastr.error(
            `Error removing coupon from shopping cart for user id ${user.id}}\r\n` + err.message,
            'Error',
          );
          throw err;
        }),
      );
  }

  public applyCoupon(couponCode: string) {
    let user: UserRecord | null = this.authService.getUser();
    if (user == null) {
      throw new Error('Cannot get a shopping cart if a user is not logged in');
    }

    let applyCouponDto: ApplyCouponDto = { userId: user.id, couponCode: couponCode };

    // this.couponService.getCouponByCode(couponCode).subscribe((response : ResponseDto) => {
    //  });

    return this.http
      .post<ResponseDto>(`${this.baseUrl}/api/cart/ApplyCouponToCart`, applyCouponDto)
      .pipe(
        tap((responseDto) => {
          if (!responseDto.isSuccess) {
            throw new Error(responseDto.message);
          }
        }),
        catchError((err) => {
          this.toastr.error(err.message, 'Error');
          throw err;
        }),
      );
  }

  public addProductToCart(productId: number, quantity: number, stockQuantity: number) {
    let user: UserRecord | null = this.authService.getUser();
    if (user == null) {
      throw new Error('Cannot get a shopping cart if a user is not logged in');
    }
    let addProductToCart: AddProductToCartDto = {
      userId: user.id,
      productId: productId,
      quantity: quantity,
      stockQuantity: stockQuantity,
    };
    return this.http
      .post<ResponseDto>(`${this.baseUrl}/api/cart/AddProductToCart`, addProductToCart)
      .pipe(
        tap((responseDto) => {
          if (!responseDto.isSuccess) {
            throw new Error(responseDto.message);
          }
        }),
        catchError((err) => {
          this.toastr.error('Cannot add product to cart<br/><br/>' + err.message);
          throw err;
        }),
      );
  }

  public removeProductFromCart(cartDetailId: number) {
    let removeProductFromCartDto: RemoveProductFromCartDto = {
      cartDetailsId: cartDetailId.toString(),
    };
    return this.http
      .post<ResponseDto>(`${this.baseUrl}/api/cart/RemoveProductFromCart`, removeProductFromCartDto)
      .pipe(
        tap((responseDto) => {
          if (!responseDto.isSuccess) {
            throw new Error(responseDto.message);
          }
          this.toastr.success('Product successfully deleted from the cart');
        }),
        catchError((err) => {
          this.toastr.error(`Error removing product from cart}\r\n` + err, 'Error');
          throw err;
        }),
      );
  }

  public emptyCart() {
    let user: UserRecord | any = this.authService.getUser();
    return this.http.delete<ResponseDto>(`${this.baseUrl}/api/cart/DeleteCart/${user.id}`).pipe(
      tap((responseDto) => {
        if (!responseDto.isSuccess) {
          throw new Error(responseDto.message);
        }
        this.toastr.success('Shopping cart has been emptied');
      }),
      catchError((err) => {
        this.toastr.error(err, 'Error');
        throw err;
      }),
    );
  }

  public deleteCart(userId: string) {
    return this.http.delete(`${this.baseUrl}/api/cart/DeleteCart/${userId}`).pipe(
      tap((ResponseDto: ResponseDto | any) => {
        if (!ResponseDto.isSuccess) {
          throw new Error(ResponseDto.message);
        }
      }),
      catchError((err) => {
        this.toastr.error(err, 'Error');
        throw err;
      }),
    );
  }

}
